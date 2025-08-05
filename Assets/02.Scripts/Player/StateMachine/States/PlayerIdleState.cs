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
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnEnterState()
    {
        _fsm.CanInteract = true;
        _fsm.CanUseItem = true;
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);

        
        if (!Mathf.Approximately(_fsm.CurrentInput.direction.sqrMagnitude, 0f))
        {
            Machine.ForceActivateState<PlayerMoveState>();
            return;
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