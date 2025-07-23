using UnityEngine;
using Fusion;
public class PlayerCookingState : APlayerState
{
    public PlayerCookingState(PlayerStateMachine fsm, PlayerController controller) : base(fsm, controller) { }

    public override void Enter()
    {
        // 애니메이션 재생 시작
        if (_controller.Object.HasInputAuthority)
        {
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Cook);
        }
    }

    public override void Exit()
    {
        CookingPanelManager.Instance.OnCookingCompleted();
        _controller.Rpc_PlayAnimTrigger(EAnimTrigger.CookDone);
    }


    public override void Tick()
    {

    }
}
