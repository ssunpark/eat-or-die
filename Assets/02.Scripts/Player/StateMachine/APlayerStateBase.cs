using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public abstract class APlayerStateBase : State<APlayerStateBase>
{
    protected PlayerFSM _fsm;
    protected StatManager _stat;
    protected ResourceManager _resource;
    protected NetworkInputData _input;
    protected TraitExpHandler _expHandler;
    public SimpleKCC KCC;
    public Animator Anim;

    protected bool _shouldPlayAnimation = true;
    public string AnimState;
    public float AnimTransitionLength = 4f / 60f;
    private bool _isLazyInitialized = false;
    

    public APlayerStateBase(PlayerFSM fsm)
    {
        _fsm = fsm;
        KCC = fsm.GetComponent<SimpleKCC>();
        Anim = fsm.GetComponent<Animator>();
    }

    protected bool IsInteractTargetExists()
    {
        return _fsm.InteractTarget != null;
    }

    protected bool IsUseItemTargetExists()
    {
        return _fsm.ItemUseTarget != null;
    }

    protected override void OnEnterState()
    {
        LazySet();
    }

    protected override void OnEnterStateRender()
    {
        LazySet();
        if (Anim != null && !string.IsNullOrEmpty(AnimState) && _shouldPlayAnimation)
        {
            Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    protected void GrantExpOrder(string actionName, int? @int = null)
    {
        _expHandler.GrantExp(actionName, @int);
    }

    protected void LazySet()
    {
        if (_isLazyInitialized) return;
        _stat ??= _fsm.PlayerNetworkObject?.Stat;
        _resource ??= _fsm.PlayerNetworkObject?.Resource;
        _expHandler ??= _fsm.PlayerNetworkObject?.ExpHandler;
        _isLazyInitialized = true;
    }

    
}