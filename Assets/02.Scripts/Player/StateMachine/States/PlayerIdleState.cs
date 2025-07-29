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
            if(CanAttack)
            {
                _fsm.ChangeState(EPlayerState.Attack);
                return;
            }
        }
        if (inputData.isInteracting)
        {
            IInteractable interactable;
            if (_fsm.Interact.TryInteract(out interactable))
            {
                _fsm.Interactable = interactable;
                _fsm.ChangeState(EPlayerState.Interact);
                return;
            }
        }
        if (inputData.isUsing)
        {
            IUsable usable;
            if (_fsm.Interact.TryUseItem(out usable))
            {
                _fsm.Usable = usable;
                _fsm.ChangeState(EPlayerState.UsingTool);
                return;
            }
        }

        Vector3 dir = inputData.direction;

        if (dir.sqrMagnitude > 0.01f)
        {
            _fsm.ChangeState(EPlayerState.Move);
        }
    }
}