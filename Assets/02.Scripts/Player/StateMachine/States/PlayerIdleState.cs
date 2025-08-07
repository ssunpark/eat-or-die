using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;

public class PlayerIdleState : APlayerStateBase
{
    public PlayerIdleState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Idle";
        StateId = (int)EPlayerState.Idle;
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }

    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
    }

    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
        var input = _fsm.CurrentInput.direction;
        if (!Mathf.Approximately(input.sqrMagnitude, 0f))
        {
            Vector3 dir = new Vector3(input.x, 0, input.y);
            KCC.SetLookRotation(Quaternion.LookRotation(dir));
            KCC.Move(dir * _stat.GetStat(EStatType.MoveSpeed));
            Machine.ForceActivateState<PlayerMoveState>();
            return;
        }
        else
        {
            KCC.Move(Vector3.zero);
        }
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

    }


}