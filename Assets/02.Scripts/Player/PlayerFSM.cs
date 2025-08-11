using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EUseItemMode : byte
{
    Self = 0,
    Give = 1,
    Targeted = 2
}
public class FSMStateInstances
{
    public PlayerIdleState Idle;
    public PlayerMoveState Move;
    public PlayerAttackState Attack;
    public PlayerUseItemState UseItem;
    public PlayerInteractState Interact;
    public PlayerHitState Hit;
    public PlayerDeadState Dead;
    public PlayerCookingState Cooking;
    public PlayerBerserkState Berserk;
    public PlayerRecoverState Recover;
    public PlayerCorpseState Corpse;
}
[RequireComponent(typeof(StateMachineController))]
public class PlayerFSM : NetworkBehaviour, IStateMachineOwner
{
    public bool EnableDebugLog = false;
    #region FSM

    private StateMachine<APlayerStateBase> _playerFSM; // 메인 상태 머신
    public StateMachine<APlayerStateBase> StateMachine => _playerFSM;
    public FSMStateInstances FSMStateInstances { get; private set; }


    #endregion

    #region Components & References
    public PlayerAnimator PlayerAnimatorController { get; private set; }
    public PlayerInteractions Interact { get; private set; }
    public PlayerItemHolder ItemHolder { get; private set; }
    public CharacterStatNetworkSync StatNetworkSync { get; private set; }

    public LayerMask attackableLayerMask;

    #endregion

    private float _lastAttackTime;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set => _lastAttackTime = value;
    }


    public Player PlayerNetworkObject;
    [Networked]
    public NetworkBool CanInteract { get; set; } = false;
    [Networked]
    public NetworkBool CanAttack { get; set; } = true;
    [Networked]
    public NetworkBool CanUseItem { get; set; } = false;

    [Networked]
    public NetworkBool IsDead { get; set; } = false;

    [Networked]
    public NetworkObject ItemUseTarget { get; set; } = null;

    [Networked]
    public NetworkObject InteractTarget { get; set; } = null;

    Collider[] _testColliders = new Collider[8];
    public LayerMask InteractLayerMask;
    public LayerMask BerserkLayerMask;
    public SimpleKCC SimpleKCC;
    private NetworkInputData _currentInput;
    private NetworkInputData _previousInput;
    public ParticleSystem HungryEffect;
    public NetworkInputData CurrentInput => _currentInput;
    public NetworkInputData PreviousInput => _previousInput;

    public const float INTERACTABLE_DISTANCE = 2f;
    public const float MAX_RAYCAST_DISTANCE = 100f;
    private const float _useItemMaxDistance = 2.0f;
    [Networked]
    public EUseItemMode UseItemMode { get; set; } = EUseItemMode.Self;

    [Networked, Capacity(8)]
    public NetworkLinkedList<NetworkObject> HitTargets { get; }

    public void CollectStateMachines(List<IStateMachine> list)
    {
        InitializeFSM();
        list.Add(_playerFSM);
    }

    public override void Spawned()
    {
        SimpleKCC = GetComponent<SimpleKCC>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        Interact = GetComponent<PlayerInteractions>();
        StatNetworkSync = GetComponent<CharacterStatNetworkSync>();
        ItemHolder = GetComponent<PlayerItemHolder>();
        PlayerNetworkObject = GetComponent<Player>();
    }

    private void InitializeFSM()
    {
        FSMStateInstances = new FSMStateInstances
        {
            Idle = new PlayerIdleState(this),
            Move = new PlayerMoveState(this),
            Attack = new PlayerAttackState(this),
            UseItem = new PlayerUseItemState(this),
            Interact = new PlayerInteractState(this),
            Hit = new PlayerHitState(this),
            Dead = new PlayerDeadState(this),
            Cooking = new PlayerCookingState(this),
            Berserk = new PlayerBerserkState(this),
            Recover = new PlayerRecoverState(this),
            Corpse = new PlayerCorpseState(this)
        };

        _playerFSM = new StateMachine<APlayerStateBase>("Player FSM",
            FSMStateInstances.Idle,
            FSMStateInstances.Move,
            FSMStateInstances.Attack, 
            FSMStateInstances.Interact,
            FSMStateInstances.UseItem, 
            FSMStateInstances.Cooking,
            FSMStateInstances.Hit,
            FSMStateInstances.Dead,
            FSMStateInstances.Berserk,
            FSMStateInstances.Recover,
            FSMStateInstances.Corpse
        );
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            if (CanInteract)
            {
                if (!TestInteraction(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Interact)))
                {
                    RPC_SetInteractTarget(null);
                }
            }

            if (CanUseItem)
            {
                if (!TestUseItem(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.UseItem)))
                {
                    RPC_SetItemUseTargetAndMode(null, EUseItemMode.Self);
                }
            }
        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetInteractTarget(NetworkObject obj)
    {
        InteractTarget = obj;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetItemUseTargetAndMode(NetworkObject obj, EUseItemMode mode)
    {
        ItemUseTarget = obj;
        UseItemMode = mode;
    }


    private bool TestUseItem(bool usePressed)
    {
        if (ItemHolder.HeldItemInstance == null)
            return false;

        string requiredTag = ItemHolder.InteractionTag;
        if (string.IsNullOrEmpty(requiredTag) || requiredTag == "Unarmed")
            return false;

        if (requiredTag == "Player")
        {
            // 커서 대상 우선
            if (TryGetPlayerUnderCursor(out var playerUnderCursor))
            {
                RPC_SetItemUseTargetAndMode(playerUnderCursor, EUseItemMode.Give);
                return true;
            }

            // 커서에 유효 대상이 없으면 자기 자신
            RPC_SetItemUseTargetAndMode(PlayerNetworkObject.Object, EUseItemMode.Self);
            return true;
        }
        Vector3 interactionPoint = transform.position + transform.forward;

        int result = Runner.GetPhysicsScene().OverlapSphere(interactionPoint, INTERACTABLE_DISTANCE, _testColliders, InteractLayerMask, QueryTriggerInteraction.Collide);

        GameObject closest = null;
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < result; i++)
        {
            GameObject obj = _testColliders[i].gameObject;

            if (!obj.CompareTag(requiredTag))
                continue;

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = obj;
            }
        }

        if (closest == null)
            return false;

        if (closest.TryGetComponent(out NetworkObject net))
        {
            RPC_SetItemUseTargetAndMode(net, EUseItemMode.Targeted);
            return true;
        }

        Debug.LogWarning($"[PlayerController] Closest usable object has no NetworkObject: {closest.name}");
        return false;
    }

    private bool TestInteraction(bool interactPressed)
    {
        if (!InputReader.Instance.HaveControl())
        {
            return false;
        }
        Vector3 interactionPoint = transform.position + transform.forward;

        int result = Runner.GetPhysicsScene().OverlapSphere(interactionPoint, 2f, _testColliders, InteractLayerMask, QueryTriggerInteraction.Collide);

        IInteractable closestInteractable = null;
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < result; i++)
        {
            if (_testColliders[i].TryGetComponent<IInteractable>(out var interactable))
            {
                float distance = Vector3.Distance(_testColliders[i].transform.position, interactionPoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable == null)
            return false;

        if (interactPressed)
        {
            if (closestInteractable.IsImmediate)
            {
                closestInteractable.Interact();
                return false;
            }
        }
        if (closestInteractable is Component comp && comp.TryGetComponent(out NetworkObject net))
        {
            RPC_SetInteractTarget(net);
            return true;
        }
        Debug.LogWarning($"[PlayerController] Interactable object does not have NetworkObject.");

        return false;

    }


    public void SetInput(NetworkInputData input)
    {
        _previousInput = _currentInput;
        _currentInput = input;
    }

    
    public void RequestActivateState(EPlayerState state)
    {
        if (HasStateAuthority)
        {
            _playerFSM.ForceActivateState((int)state);
        }
        else if(HasInputAuthority)
        {
                RPC_RequestChangeStates(state);
        }
        
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestChangeStates(EPlayerState state)
    {
        _playerFSM.ForceActivateState((int)state);
    }

    private bool TryGetPlayerUnderCursor(out NetworkObject targetPlayer)
    {
        targetPlayer = null;
        var cam = Camera.main;
        if (cam == null) return false;

        var ray = cam.ScreenPointToRay(Input.mousePosition);
        var scene = Runner.GetPhysicsScene();
#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * MAX_RAYCAST_DISTANCE, Color.red, 1f);
#endif
        if (EnableDebugLog)
        {
            Debug.Log($"[PlayerFSM] Raycasting from {ray.origin} in direction {ray.direction} for {MAX_RAYCAST_DISTANCE} units.");
        }
        if (scene.Raycast(ray.origin, ray.direction, out RaycastHit hit, MAX_RAYCAST_DISTANCE, InteractLayerMask, QueryTriggerInteraction.Collide))
        {
            var go = hit.collider.gameObject;
            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] Hit object: {go.name} at distance {hit.distance}");
            }
            var player = hit.collider.GetComponentInParent<Player>();
            if (player == null) return false;

            var netObj = player.NetworkObject;
            if (netObj == null) return false;

            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] NetworkObject found: {netObj.name}");
            }
            var dist = Vector3.Distance(transform.position, netObj.transform.position);
            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] Distance to player: {dist}");
            }
            if (dist <= _useItemMaxDistance)
            {
                if (EnableDebugLog)
                {
                    Debug.Log($"[PlayerFSM] Player is within use item distance: {dist}");
                }
                targetPlayer = netObj;
                return true;
            }
        }
        if (EnableDebugLog)
        {
            Debug.Log($"[PlayerFSM] No player found under cursor.");
        }
        return false;
    }

    public void RequestRequestPlayParticle(string key, Vector3 worldPos, Quaternion rot)
    {
        ParticleManager.Instance.RPC_RequestPlayParticle(key, worldPos, rot);
    }
}
