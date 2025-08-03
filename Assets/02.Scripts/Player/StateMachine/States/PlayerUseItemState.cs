using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerUseItemState(PlayerFSM controller) : base(controller)
    {
        AnimState = "UseItem";
    }
    protected override void OnEnterStateRender()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }


    void IAnimationActionNotify.OnActionMoment()
    {
        if (_fsm.HasInputAuthority)
        {
            Debug.Log("PlayerUseItemState: Using item at action moment.");
            _fsm.ItemHolder.UseItem(_fsm.ItemUseTarget.gameObject);
        }
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            Machine.ForceActivateState<PlayerIdleState>();
        }
    }
}