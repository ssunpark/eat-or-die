using UnityEngine;
using Fusion; // Add Fusion for NetworkInputData

public class PlayerInteractState : APlayerState
{
    public PlayerInteractState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller)
    {
    }

    public override void Enter()
    {
        // Logic to handle entering the interact state
        Debug.Log("Entering Player Interact State");
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