using Fusion.Addons.FSM;
using UnityEngine; // Add Fusion FSM for PlayerStateMachine
public class PlayerInteractState : APlayerStateBase, IAnimationActionNotify, IAnimationActionEndNotify
{
    public PlayerInteractState(PlayerController controller) : base(controller)
    {
        StateId = (int)EPlayerState.Interact;
    }

    private GameObject _target;
    private bool _animationFinished;
    protected override void OnInitialize()
    {
        this.AddTransition(
            _controller.FSMStateInstances.Idle,
            () => _animationFinished
        );
    }
    protected override void OnEnterState()
    {
        _animationFinished = false;
        _controller.PlayAnimTriggerNetwork(EAnimTrigger.Interact);
        if (_controller.CanInteract(out GameObject target))
        {
            _target = target;
        }
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        _controller.Interact.Interact(_target);
    }

    void IAnimationActionEndNotify.OnAnimationFinished()
    {
        _animationFinished = true;
    }
}