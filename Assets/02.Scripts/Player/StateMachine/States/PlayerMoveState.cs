using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public class PlayerMoveState : APlayerStateBase
{
    private float _hungerConsumptionOvertime; 
    private float _moveSpeed;
    private float _sprintMultipler;
    private float _moveExpTimer;
    private TraitExpHandler _expHandler;
    public PlayerMoveState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Move";
        StateId = (int)EPlayerState.Move;
        
    }


    protected override void OnEnterState()
    {
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }

        if(_stat == null || _resource == null)
        {
            Debug.LogError("PlayerMoveState: Stat or Resource is null. Cannot enter state.");
            return;
        }
        _hungerConsumptionOvertime = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.HungerConsumptionOverTime);
        _moveSpeed = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.SprintingMultiplier);
        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
        if(_expHandler == null)
        {
            _expHandler = _fsm.PlayerNetworkObject.ExpHandler;
        }
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        float multiplier = _fsm.CurrentInput.buttons.IsSet(EButtons.Run) ? _sprintMultipler : 1f;

        var moveInput = _fsm.CurrentInput.direction;

        if (moveInput.sqrMagnitude < 0.01f)
        {
            Machine.ForceActivateState<PlayerIdleState>();
            KCC.Move(Vector3.zero);
            return;
        }
        _moveExpTimer += Machine.Runner.DeltaTime;
        if (_moveExpTimer >= 1f)
        {
            //Debug.Log("[PlayerMoveState] Granting MovePerSecond experience.");
            _expHandler.GrantExp("MovePerSecond");
            _moveExpTimer = 0f;
        }

        Vector2 normalized = moveInput.normalized;
        Vector3 movementDirection = new Vector3(normalized.x, 0, normalized.y);

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            KCC.SetLookRotation(Quaternion.LookRotation(movementDirection));
        }

        KCC.Move(movementDirection * _moveSpeed * multiplier, 0);
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Attack))
        {
            Machine.ForceActivateState<PlayerAttackState>();
            return;
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.Interact))
        {
            if (IsInteractTargetExists())
            {
                Machine.ForceActivateState<PlayerInteractState>();
                return;
            }
        }
        if (_fsm.CurrentInput.buttons.WasPressed(_fsm.PreviousInput.buttons, EButtons.UseItem))
        {
            if (IsUseItemTargetExists())
            {
                Machine.ForceActivateState<PlayerUseItemState>();
                return;
            }
        }
        _resource.ConsumeHunger(_hungerConsumptionOvertime * Machine.Runner.DeltaTime);

    }

    protected override void OnExitState()
    {
    }
}