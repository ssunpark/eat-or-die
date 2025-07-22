using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData

public class PlayerIdleState : APlayerState
{
    public PlayerIdleState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override bool CanMove => true;

    public override bool CanAct => true;
    public override void Tick()
    {
        if (!_controller.GetInput(out NetworkInputData inputData)) return;
        
        if (inputData.isAttacking)
        {
            _fsm.ChangeState(EPlayerState.Attack);
            return;
        }

        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude > 0.01f)
        {
            _fsm.ChangeState(EPlayerState.Move);
        }
    }
}