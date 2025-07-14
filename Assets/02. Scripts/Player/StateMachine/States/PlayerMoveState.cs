using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }
    public override void Enter()
    {
        //controller.PlayerAnimatorController.SetBool("IsMoving", true);
    }

    public override void Tick()
    {
        if (!controller.GetInput(out var inputData)) return;

        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude > 0.01f)
        {
            bool isRunning = inputData.isRunning;
            float moveSpeed = isRunning ? controller.RunSpeed : controller.WalkSpeed;

            controller.Move(dir, moveSpeed);
            controller.Animator.SetFloat("MoveSpeed", isRunning ? 1f : 0.5f);
        }
        else
        {
            fsm.ChangeState(EPlayerState.Idle);
        }
    }

    public override void Exit()
    {
        controller.Animator.SetBool("IsMoving", false);
    }
}