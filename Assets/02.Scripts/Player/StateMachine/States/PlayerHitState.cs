using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using Unity.VisualScripting;
public class PlayerHitState : APlayerStateBase
{
    public PlayerHitState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Hit";
        delayTime = 0.33333f;
    }
    public float delayTime;
    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
    }

    protected override void OnEnterStateRender()
    {
        if (!_fsm.HasStateAuthority)
        {
            //_fsm.PlayerNetworkObject.damageFX.PlayFX();
        }
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {

        if (Machine.StateTime >= delayTime)
        {
            Machine.ForceActivateState<PlayerIdleState>();
            return;
        }
    }
}
