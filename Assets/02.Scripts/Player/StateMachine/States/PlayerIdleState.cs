using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine

public class PlayerIdleState : APlayerStateBase
{
    public PlayerIdleState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Idle;
    }

    protected override void OnEnterState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.RPC_SetMoveFlag(false);
        }
    }
    protected override void OnFixedUpdate()
    {
        if (!_controller.GetInput(out NetworkInputData input))
            return;

        if (PlayerFSMTransitionEvaluator.Evaluate(_controller, input, Machine.Runner, out var next))
        {
            Machine.ForceActivateState(next);
            return;
        }

    }

    
}