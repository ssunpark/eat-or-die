using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerUseItemState(PlayerFSM controller) : base(controller)
    {
        AnimState = "UseItem";
    }

    private NetworkObject _target;
    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }
        if (!ValidateItemUseTarget())
        {
            return;
        }

    }

    protected override void OnEnterStateRender()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
        if (!ValidateItemUseTarget())
        {
            return;
        }
        _target = _fsm.ItemUseTarget;
    }

    /// <summary>
    /// Checks if ItemUseTarget is not null, logs error if it is.
    /// </summary>
    /// <returns>True if ItemUseTarget is not null, false otherwise.</returns>
    private bool ValidateItemUseTarget()
    {
        if (_fsm.ItemUseTarget == null)
        {
            Debug.LogError("PlayerUseItemState: ItemUseTarget is null. Cannot enter state.");
            return false;
        }
        return true;
    }
    void IAnimationActionNotify.OnActionMoment()
    {
        if (_fsm.HasInputAuthority)
        {
            Debug.Log("PlayerUseItemState: Using item at action moment.");
            if (_target == null)
            {
                Debug.LogWarning("PlayerUseItemState: Target is null. Cannot use item.");
                return;
            }
            _fsm.ItemHolder.UseItem(_target.gameObject);
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