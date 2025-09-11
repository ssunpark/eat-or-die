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
        const int corpseItemId = 1800001;
        var position = _fsm.transform.position;
        var rotation = Quaternion.identity;
        var ownerId = CharacterInfoManager.Instance.CharacterInfo.Id;
        ItemProxySpawner.Instance.RPC_CreateItemObject(
            corpseItemId,
            1,
            0f,
            position,
            rotation,
            ownerId);
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