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
        _fsm.PlayerNetworkObject.InstantRevive();
    }

    protected override void OnEnterStateRender()
    {
        _fsm.HeadCanvas.SetActive(false);

    }

    protected override void OnExitStateRender()
    {
        _fsm.HeadCanvas.SetActive(true);
    }

    protected override void OnFixedUpdateInput()
    {
    }
}