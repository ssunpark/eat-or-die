using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public abstract class ABerserkSubStateBase : State<ABerserkSubStateBase>, IAnimationActionNotify
{
    protected PlayerFSM _fsm;

    protected StatManager _stat;
    protected ResourceManager _resource;
    public SimpleKCC KCC;
    public Animator Anim;
    public string AnimState;
    public float AnimTransitionLength = 4f / 60f;
    protected ABerserkSubStateBase(PlayerFSM fsm)
    {
        _fsm = fsm;
        _stat = fsm.PlayerNetworkObject?.Stat;
        _resource = fsm.PlayerNetworkObject?.Resource;
        KCC = fsm.GetComponent<SimpleKCC>();
        Anim = fsm.GetComponent<Animator>();
    }

    protected void LazySet()
    {
        if (_fsm == null)
        {
            Debug.LogError("PlayerFSM is null. Cannot set state.");
            return;
        }
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }
    }

    public abstract void OnActionMoment();
}