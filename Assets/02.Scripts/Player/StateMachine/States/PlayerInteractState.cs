using Fusion;
using Fusion.Addons.FSM;
using UnityEngine; // Add Fusion FSM for PlayerStateMachine
public class PlayerInteractState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerInteractState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Interact";
        StateId = (int)EPlayerState.Interact;
    }
    private NetworkObject _target;
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();

        if (_fsm.InteractTarget == null)
        {
            return;
        }
        _target = _fsm.InteractTarget;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        if (_fsm.InteractTarget == null)
        {
            Debug.LogError("PlayerInteractState: Target is null. Cannot enter state.");
            Machine.ForceActivateState<PlayerIdleState>();
            return;
        }
        _target = _fsm.InteractTarget;
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
    void IAnimationActionNotify.OnActionMoment()
    {
        if (_fsm.HasStateAuthority)
        {
            if (_fsm.InteractTarget == null)
            {
                Debug.LogWarning("PlayerInteractState: Interact target is null. Cannot perform interaction.");
                return;
            }
            _fsm.Interact.Interact(_target);
        }
    }

}