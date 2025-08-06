using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using Fusion;
public class PlayerHitState : APlayerStateBase
{
    public PlayerHitState(PlayerFSM controller) : base(controller)
    {
        AnimState = "Hit";
        delayTime = 0.33333f;
        StateId = (int)EPlayerState.Hit;
    }
    public float delayTime;
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();

        if (!_fsm.HasStateAuthority)
        {
            //_fsm.PlayerNetworkObject.damageFX.PlayFX();
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;
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
