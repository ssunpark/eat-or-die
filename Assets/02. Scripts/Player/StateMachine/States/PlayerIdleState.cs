using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override void Tick()
    {
        if (!controller.GetInput(out NetworkInputData inputData)) return;

        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude > 0.01f)
        {
            fsm.ChangeState(EPlayerState.Move);
        }
    }
}
