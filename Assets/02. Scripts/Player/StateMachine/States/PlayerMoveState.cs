using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }
    public override void Enter()
    {
    }

    public override void Tick()
    {
        if (!_controller.GetInput(out NetworkInputData inputData)) return;

        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude <= 0.01f)
        {
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
        _controller.Stop();
    }
}