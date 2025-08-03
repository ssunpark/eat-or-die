
using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StateValues
{
    public float MoveHungerTimer = 0f;
    public float MoveHungerInterval = 1f;
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
}
[RequireComponent(typeof(StateMachineController))]
public class PlayerFSM : NetworkBehaviour, IStateMachineOwner
{

    #region FSM

    private StateMachine<APlayerStateBase> _playerFSM; // 메인 상태 머신
    public StateMachine<APlayerStateBase> StateMachine => _playerFSM;
    public FSMStateInstances FSMStateInstances { get; private set; }
    public StateValues StateValues { get; set; } = new StateValues();


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
    public SimpleKCC SimpleKCC;
    private NetworkInputData _currentInput;
    private NetworkInputData _previousInput;

    public NetworkInputData CurrentInput => _currentInput;
    public NetworkInputData PreviousInput => _previousInput;

    public const float INTERACTABLE_DISTANCE = 2f;
    public const float MAX_RAYCAST_DISTANCE = 100f;
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
            Recover = new PlayerRecoverState(this)
        };

        _playerFSM = new StateMachine<APlayerStateBase>("Player FSM",
            FSMStateInstances.Idle,
            FSMStateInstances.Move,
            FSMStateInstances.Attack,
            FSMStateInstances.UseItem,
            FSMStateInstances.Interact,
            FSMStateInstances.Hit,
            FSMStateInstances.Dead,
            FSMStateInstances.Cooking,
            FSMStateInstances.Berserk
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
                    RPC_SetItemUseTarget(null);
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
    private void RPC_SetItemUseTarget(NetworkObject obj)
    {
        ItemUseTarget = obj;
    }


    private bool TestUseItem(bool usePressed)
    {
        if (ItemHolder.HeldItem == null)
            return false;

        string requiredTag = ItemHolder.InteractionTag;
        if (string.IsNullOrEmpty(requiredTag) || requiredTag == "Undefined")
            return false;

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
            RPC_SetItemUseTarget(net);
            return true;
        }

        Debug.LogWarning($"[PlayerController] Closest usable object has no NetworkObject: {closest.name}");
        return false;
    }

    private bool TestInteraction(bool interactPressed)
    {
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

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Attack: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Attack)}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Move: {CurrentInput.buttons.WasReleased(PreviousInput.buttons, EButtons.Attack)}");
        GUI.Label(new Rect(10, 50, 200, 20), $"Interact: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Interact)}");
        GUI.Label(new Rect(10, 70, 200, 20), $"UseItem: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.UseItem)}");

    }

    public void SetInput(NetworkInputData input)
    {
        _previousInput = _currentInput;
        _currentInput = input;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            return;

        if (StateMachine?.ActiveState is PlayerAttackState attackState)
        {
            Vector3 origin = transform.position + transform.rotation * new Vector3(0f, 0.2f, 0.5f);
            float range = AttackRange;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin, range);
        }
#endif
    }

    public float AttackRange => PlayerNetworkObject.Stat.GetStat(EStatType.AttackRange);
}
