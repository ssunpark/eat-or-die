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
    public PlayerMoveState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Move";
        StateId = (int)EPlayerState.Move;
    }


    protected override void OnEnterState()
    {
        base.OnEnterState();
        _hungerConsumptionOvertime = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.HungerConsumptionOverTime);
        _moveSpeed = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _fsm.PlayerNetworkObject.Stat.GetStat(EStatType.SprintingMultiplier);
        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
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
            GrantExpOrder("MovePerSecond");
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