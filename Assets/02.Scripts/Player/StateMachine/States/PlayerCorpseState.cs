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
        
    }

    protected override void OnEnterStateRender()
    {
        _fsm.PlayerNetworkObject.HideCharacter(true, true);
        if (_fsm.HasInputAuthority)
        {
            _fsm.PlayerNetworkObject.CameraFollow.RebuildTargets();
            _fsm.PlayerNetworkObject.CameraFollow.EnableSpectator();
            _fsm.ShowSpectatorPanel();
        }
    }

    protected override void OnExitStateRender()
    {
        _fsm.PlayerNetworkObject.HideCharacter(false, true);
        if (_fsm.HasInputAuthority)
        {
            _fsm.PlayerNetworkObject.CameraFollow.DisableSpectator();
            _fsm.HideSpectatorPanel();
        }
    }

    protected override void OnFixedUpdateInput()
    {
    }
}