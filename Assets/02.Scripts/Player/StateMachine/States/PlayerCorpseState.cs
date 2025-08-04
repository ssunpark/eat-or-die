using UnityEngine;
public class PlayerCorpseState : APlayerStateBase
{
    public PlayerCorpseState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Die";
        StateId = (int)EPlayerState.Corpse;
    }
    private Renderer[] _rendererObjects;
    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _fsm.IsDead = true;
    }

    protected override void OnEnterStateRender()
    {
        _rendererObjects = _fsm.GetComponentsInChildren<Renderer>(true);
        foreach(var renderer in _rendererObjects)
            renderer.gameObject.SetActive(false);
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnExitStateRender()
    {
        foreach (var renderer in _rendererObjects)
            renderer.gameObject.SetActive(true);
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);
    }
}