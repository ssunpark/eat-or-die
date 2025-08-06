using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerUseItemState(PlayerFSM controller) : base(controller)
    {
        AnimState = "UseItem";
        StateId = (int)EPlayerState.UseItem;
    }

    private NetworkObject _target;
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        if(_fsm.ItemUseTarget == null)
        {
            Machine.ForceActivateState<PlayerIdleState>();
        }

    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        if (!_fsm.HasStateAuthority) return;
        RPC_UseItemOrder(_target);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    protected void RPC_UseItemOrder(NetworkObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("PlayerUseItemState: Target is null. Cannot use item.");
            return;
        }
        _fsm.ItemHolder.UseItem(target.gameObject);
    }
    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            Machine.ForceActivateState<PlayerIdleState>();
        }
    }
}