using UnityEngine;
public class PlayerRecoverState : APlayerStateBase
{

    public PlayerRecoverState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Recover";
        StateId = (int)EPlayerState.Recover;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _resource.RestoreHunger(_resource.MaxHunger / 20);
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
