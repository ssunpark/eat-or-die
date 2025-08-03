using Fusion.Addons.FSM;
using UnityEngine; // Add Fusion FSM for PlayerStateMachine
public class PlayerInteractState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerInteractState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Interact";
    }

    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            Machine.ForceActivateState<PlayerIdleState>();
        }
    }
    void IAnimationActionNotify.OnActionMoment()
    {
        if(_fsm.HasInputAuthority)
            _fsm.Interact.Interact(_fsm.InteractTarget);
    }

}