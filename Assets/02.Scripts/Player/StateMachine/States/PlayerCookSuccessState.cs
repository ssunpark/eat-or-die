using UnityEngine;
public class PlayerCookSuccessState : APlayerStateBase
{

    public PlayerCookSuccessState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Cook_Success";
        StateId = (int)EPlayerState.CookSuccess;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
    }

    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            RequestActivateState();
        }
    }
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }
    protected override void OnExitState()
    {
    }

}
