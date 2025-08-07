using UnityEngine;
public class PlayerCorpseState : APlayerStateBase
{
    public PlayerCorpseState(PlayerFSM fsm) : base(fsm)
    {
        AnimState = "Die";
        StateId = (int)EPlayerState.Corpse;
        AnimTransitionLength = 0;
    }
    private Renderer[] _rendererObjects;
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        _fsm.IsDead = true;
    }

    protected override void OnEnterStateRender()
    {
        _rendererObjects = _fsm.GetComponentsInChildren<Renderer>(true);
        //Todo: 활성화 되어있던 오브젝트들 저장
        foreach(var renderer in _rendererObjects)
            renderer.gameObject.SetActive(false);
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnExitStateRender()
    {
        //Todo: 활성화 되어있던 오브젝트들 복원
        foreach (var renderer in _rendererObjects)
            renderer.gameObject.SetActive(true);
    }

    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);
    }
}