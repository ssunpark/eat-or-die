using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
public class PlayerCookingState : APlayerStateBase
{
    public PlayerCookingState(PlayerFSM controller) : base(controller) 
    {
        AnimState = "Cook";
        StateId = (int)EPlayerState.Cooking;
    }
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;

    }
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }

    protected override void OnFixedUpdate()
    {
        if (!_fsm.HasStateAuthority) return;

        KCC.Move(Vector3.zero);

        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            RPC_NotifyCookingComplete();

            Machine.ForceActivateState<PlayerIdleState>();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_NotifyCookingComplete()
    {
        // 요리 완료 처리
        CookingManager.Instance.OnCookingCompleted();
    }
}
