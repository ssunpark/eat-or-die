using UnityEngine;
public class PlayerCorpseState : APlayerStateBase
{
    public PlayerCorpseState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Die";
        StateId = (int)EPlayerState.Corpse;
        AnimTransitionLength = 0;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _fsm.IsDead = true;
        //_fsm.PlayerNetworkObject.InstantRevive();
    }

    protected override void OnEnterStateRender()
    {
        _fsm.HeadCanvas.SetActive(false);
        _fsm.RenderModel.SetActive(false);
        if (_fsm.HasInputAuthority)
        {
            _fsm.PlayerNetworkObject.CameraFollow.RebuildTargets();
            _fsm.PlayerNetworkObject.CameraFollow.EnableSpectator();
        }
    }

    protected override void OnExitStateRender()
    {
        _fsm.HeadCanvas.SetActive(true);

        _fsm.RenderModel.SetActive(true);
        if (_fsm.HasInputAuthority)
        {
            _fsm.PlayerNetworkObject.CameraFollow.DisableSpectator();
        }
    }

    protected override void OnFixedUpdateInput()
    {
    }
}