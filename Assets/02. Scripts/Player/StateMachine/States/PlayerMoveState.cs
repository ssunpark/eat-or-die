using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private PlayerStats _stat;
    private PlayerAnimator _animator;
    public PlayerMoveState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
        _stat = controller.PlayerStats;
        _animator = controller.PlayerAnimatorController;
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
            float moveSpeed = isRunning ? _stat.RunSpeed : _stat.WalkSpeed;
            _controller.Move(dir, moveSpeed);
            //_animator.SetMoveSpeed(isRunning ? 1f : 0.5f);
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