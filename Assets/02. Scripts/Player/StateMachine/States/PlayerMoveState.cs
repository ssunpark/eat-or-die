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
        TryJump(inputData);
        Vector3 dir = inputData.direction;

        
        if (dir.sqrMagnitude > 0.01f)
        {
            bool isRunning = inputData.isRunning;
            float baseSpeed = _stat.GetStat(EStatType.MoveSpeed);
            float sprintMultiplier = inputData.isRunning
                ? _stat.GetStat(EStatType.SprintingMultiplier)
                : 1f;

            float moveSpeed = baseSpeed * sprintMultiplier;
            _controller.Move(dir, moveSpeed);
        }
        else
        {
            _fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
        _controller.Stop();
    }
}