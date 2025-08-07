using UnityEngine;
public class PlayerCookingState : APlayerStateBase
{
    public PlayerCookingState(PlayerFSM controller) : base(controller) 
    {
        AnimState = "Cook";
        StateId = (int)EPlayerState.Cooking;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;

    }
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }

    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);

        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState]*3)
        {
            CookingManager.Instance.OnCookingCompleted();

            RequestActivateState();
        }
    }
}
