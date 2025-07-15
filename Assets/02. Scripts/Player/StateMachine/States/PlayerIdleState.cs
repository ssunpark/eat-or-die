using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override void Tick()
    {
        if (!_controller.GetInput(out NetworkInputData inputData)) return;
        TryJump(inputData);
        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude > 0.01f)
        {
            _fsm.ChangeState(EPlayerState.Move);
        }
    }
}
