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
        _fsm.PlayerNetworkObject.ItemHolder.HeldItemObject?.SetActive(false);
        if (_fsm.InteractTarget == null)
        {
            return;
        }
        if(_fsm.HasStateAuthority || _fsm.HasInputAuthority)
            _target = _fsm.InteractTarget;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        if (_fsm.InteractTarget == null)
        {
            RequestActivateState();
            return;
        }
        
    }
    protected override void OnFixedUpdateInput()
    {
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {

            _fsm.PlayerNetworkObject.ItemHolder.HeldItemObject?.SetActive(true);
            RequestActivateState();
        }
    }

    protected override void PostFixedUpdate()
    {
        if (Machine.StateTime <= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            Vector3 lookDir = _fsm.InteractTarget != null
                ? (_fsm.InteractTarget.transform.position - _fsm.transform.position).normalized
                : Vector3.forward;
            lookDir.y = 0f;
            KCC.SetLookRotation(Quaternion.LookRotation(lookDir));
            KCC.Move(Vector3.zero);
        }
    }
    void IAnimationActionNotify.OnActionMoment()
    {
        if (_fsm.HasInputAuthority)
        {
            if (_fsm.InteractTarget == null)
            {
                Debug.LogWarning("PlayerInteractState: Interact target is null. Cannot perform interaction.");
                return;
            }
            GrantExpOrder("HarvestPlant");
            _fsm.Interact.Interact(_target, _fsm.PlayerNetworkObject);
        }
    }

}