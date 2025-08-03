
using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

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
    public PlayerMove Movement { get; private set; }
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

    public void Awake()
    {
        SimpleKCC = GetComponent<SimpleKCC>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        Interact = GetComponent<PlayerInteractions>();
        StatNetworkSync = GetComponent<CharacterStatNetworkSync>();
        ItemHolder = GetComponent<PlayerItemHolder>();
        PlayerNetworkObject = GetComponent<Player>();
    }

    public void CollectStateMachines(List<IStateMachine> list)
    {
        InitializeFSM();
        list.Add(_playerFSM);
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
        if (CanInteract)
        {
            if(!TestInteraction(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Interact)))
            {
                InteractTarget = null;
            }
        }

        if (CanUseItem)
        {
            if (!TestUseItem(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.UseItem)))
            {
                ItemUseTarget = null;
            }
        }
    }

    private bool TestUseItem(bool usePressed)
    {
        if (ItemHolder.HeldItem == null)
            return false;
        string requiredTag = ItemHolder.InteractionTag;
        Vector3 interactionPoint = transform.position + transform.forward;

        if (string.IsNullOrEmpty(requiredTag) || requiredTag == "Undefined")
        {
            //Debug.Log($"[PlayerController] requiredTag is {requiredTag}.");
            // 사용아이템의 상호작용가능한 물체가 Untagged일 수도 있어서 임시로 Undefined일때 체크
            return false;
        }

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, MAX_RAYCAST_DISTANCE, InteractLayerMask))
        {
            //Debug.Log($"[PlayerController] Raycast에서 검출된 오브젝트 없음.");
            return false;
        }

        GameObject hitObject = hit.collider.gameObject;
        if (!hitObject.CompareTag(requiredTag))
        {
            //Debug.Log($"[PlayerController] hitObject: {hitObject.name}, {hitObject.tag}");
            return false;
        }
        float dist = Vector3.Distance(transform.position, hitObject.transform.position);
        if (dist > INTERACTABLE_DISTANCE)
        {
            //Debug.Log($"[PlayerController] 거리 초과: {dist} > {INTERACTABLE_DISTANCE}");
            return false;
        }

        //Debug.Log($"[PlayerController] CanUseHeldItem 성공: {hitObject.name}, 거리: {dist}");

        if (hitObject.TryGetComponent(out NetworkObject net))
        {
            ItemUseTarget = net;
            return true;
        }
        else
        {
            Debug.LogWarning($"[PlayerController] Hit object {hitObject.name} does not have a NetworkObject component.");
            return false;
        }
    }

    private bool TestInteraction(bool interactPressed)
    {
        Vector3 interactionPoint = transform.position + transform.forward;

        int result = Runner.GetPhysicsScene().OverlapSphere(interactionPoint, 2f, _testColliders, InteractLayerMask, QueryTriggerInteraction.Collide);

        int closestIndex = -1;
        float shortestDistance = float.MaxValue;
        for (int i = 0; i < result; i++)
        {
            float distance = Vector3.Distance(_testColliders[i].transform.position, interactionPoint);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestIndex = i;
            }
        }

        if (closestIndex < 0)
        {
            return false;
        }

        if (_testColliders[closestIndex].TryGetComponent<IInteractable>(out var interactable))
        {
            if (interactPressed)
            {
                // 즉시 상호작용가능한 오브젝트라면
                if (false) // IInteractable에서 즉시 상호작용 가능한지 여부를 확인하는 필드 필요
                {
                    interactable.Interact();
                    return false;
                    // 애니메이션 없이 상호작용할거라 InteractState로 안빠질겁니다
                }
                // else {
                if(_testColliders[closestIndex].TryGetComponent(out NetworkObject net))
                {
                    InteractTarget = net;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"[PlayerController] Hit object {_testColliders[closestIndex].name} does not have a NetworkObject component.");
                    return false;
                }
                //}
            }
        }
        return false;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Attack: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Attack)}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Move: {CurrentInput.buttons.WasReleased(PreviousInput.buttons,EButtons.Attack)}");
        GUI.Label(new Rect(10, 50, 200, 20), $"Interact: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Interact)}");
        GUI.Label(new Rect(10, 70, 200, 20), $"UseItem: {CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.UseItem)}");

    }
















    

    

    public void SetInput(NetworkInputData input)
    {
        _previousInput = _currentInput;
        _currentInput = input;
    }
}
