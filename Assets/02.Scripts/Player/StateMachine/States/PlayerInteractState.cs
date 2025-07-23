using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData

public class PlayerInteractState : APlayerState
{
    public PlayerInteractState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {

    }

    public override void Enter()
    {
        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Interact);
        }
    }

    public override void Tick()
    {
        
    }

    public override void Exit()
    {
        // Logic to handle exiting the interact state
        Debug.Log("Exiting Player Interact State");
    }
}