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
    protected SkillManager _skill;
    public SimpleKCC KCC;
    public Animator Anim;
    protected ActorContextHolder _skillContext;

    protected bool _shouldPlayAnimation = true;
    public string AnimState;
    public float AnimTransitionLength = 4f / 60f;
    private bool _isLazyInitialized = false;
    

    public APlayerStateBase(PlayerFSM fsm)
    {
        _fsm = fsm;
        KCC = fsm.GetComponent<SimpleKCC>();
        Anim = fsm.GetComponent<Animator>();
        _skillContext = fsm.GetComponent<ActorContextHolder>();
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

    protected override void OnFixedUpdate()
    {
        PreFixedUpdate();

        if (_fsm.HasInputAuthority)
            OnFixedUpdateInput();
        if (_fsm.HasStateAuthority)
            OnFixedUpdateState();

        PostFixedUpdate();
    }

    /// <summary>
    /// Input, State, Observer 상관없이 모든 플레이어 상태에서 고정 업데이트 전에 실행되는 메서드입니다.
    /// </summary>
    protected virtual void PreFixedUpdate() { }

    /// <summary>
    /// Input, State, Observer 상관없이 모든 플레이어 상태에서 고정 업데이트 후에 실행되는 메서드입니다.
    /// </summary>
    protected virtual void PostFixedUpdate() { }

    /// <summary>
    /// Input Authority가 있는 플레이어 상태에서 고정 업데이트 중에 실행되는 메서드입니다.
    /// </summary>
    protected virtual void OnFixedUpdateInput() { }

    /// <summary>
    /// State Authority가 있는 플레이어 상태에서 고정 업데이트 중에 실행되는 메서드입니다.
    /// </summary>
    protected virtual void OnFixedUpdateState() { }

    protected void GrantExpOrder(string actionName, int? @int = null)
    {
        if (@int.HasValue) {
            Debug.LogError("담당자: 남경민!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
        _fsm.RPC_GrantExpOrder(_fsm.Object.InputAuthority, actionName);
    }

    protected void LazySet()
    {
        if (_isLazyInitialized) return;
        _stat ??= _fsm.PlayerNetworkObject?.Stat;
        _resource ??= _fsm.PlayerNetworkObject?.Resource;
        _expHandler ??= _fsm.PlayerNetworkObject?.ExpHandler;
        _skill ??= _fsm.PlayerNetworkObject?.Skill;
        _skillContext ??= _fsm.GetComponent<ActorContextHolder>();
        _isLazyInitialized = true;
    }

    
    protected void RequestActivateState(EPlayerState state = EPlayerState.Idle)
    {
        _fsm.RequestActivateState(state);
    }

}