using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public abstract class APlayerStateBase : State<APlayerStateBase>
{
    protected PlayerFSM _fsm;
    protected StatManager _stat;
    protected ResourceManager _resource;
    protected bool _shouldAbortStateEarly = false; 
    protected NetworkInputData _input;

    public SimpleKCC KCC;
    public Animator Anim;
    public string AnimState;
    public float AnimTransitionLength = 4f / 60f;

    public APlayerStateBase(PlayerFSM fsm)
    {
        _fsm = fsm;
        _stat = fsm.PlayerNetworkObject?.Stat;
        _resource = fsm.PlayerNetworkObject?.Resource;
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

    
}