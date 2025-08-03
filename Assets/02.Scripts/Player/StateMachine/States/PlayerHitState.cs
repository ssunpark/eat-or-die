using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
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
        if (_stat == null)
        {
            _stat = _fsm.PlayerNetworkObject.Stat;
        }
        if (_resource == null)
        {
            _resource = _fsm.PlayerNetworkObject.Resource;
        }

        if (_stat == null || _resource == null)
        {
            Debug.LogError("PlayerHitState: Stat or Resource is null. Cannot enter state.");
            return;
        }
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
            if(_resource.GetHungerPercent() <= 0.1f)
            {
                Machine.ForceActivateState<PlayerBerserkState>();
                return;
            }
            Machine.ForceActivateState<PlayerIdleState>();
            return;
        }
    }
}
