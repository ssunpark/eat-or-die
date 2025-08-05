using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public class PlayerCookingState : APlayerStateBase
{
    public PlayerCookingState(PlayerFSM controller) : base(controller) 
    {
        AnimState = "Cooking";
        StateId = (int)EPlayerState.Cooking;
    }
    protected override void OnEnterState()
    {
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;

    }
    protected override void OnEnterStateRender()
    {
        Anim.CrossFadeInFixedTime("Cooking", AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            if (_fsm.Object.HasInputAuthority)
            {
                // 요리 완료 처리 CookingManager.Instance.OnCookingCompleted();
            }
            Machine.ForceActivateState<PlayerIdleState>();
        }
    }
}
