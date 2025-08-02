using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
public class PlayerBerserkState : APlayerStateBase, IAnimationActionEndNotify
{
    private StateMachine<ABerserkSubStateBase> _subFSM;
    private readonly BerserkChase _chase;
    private readonly BerserkAttack _attack;

    public PlayerBerserkState(PlayerController controller) : base(controller)
    {
        _subFSM = new StateMachine<ABerserkSubStateBase>("BerserkFSM",
            new BerserkIdle(controller), // 초기 상태
            new BerserkChase(controller),
            new BerserkAttack(controller)
        );
        _subFSM.SetDefaultState(0);
        StateId = (int)EPlayerState.Berserk;
    }
    protected override void CollectChildStateMachines(List<IStateMachine> list)
    {
        list.Add(_subFSM);
    }
    protected override bool CanExitState(IState nextState)
    {
        return _resource.GetHungerPercent() > 0.1f || _resource.GetHungerPercent()==0f;
    }
    protected override void OnEnterState()
    {
        _controller.Movement.Move(Vector3.zero, false); // 이동 멈춤

        _subFSM.Reset(); // 내부 상태머신 초기화

        if (_controller.Object.HasInputAuthority)
        {
            InputReader.playerControllerInputBlocked = true;
        }
        _controller.PlayAnimTrigger(EAnimTrigger.Berserk);
    }

    protected override void OnFixedUpdate()
    {
        _resource.ConsumeHunger(Machine.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime) * 5);
    }

    protected override void OnExitState()
    {
        if (_controller.Object.HasInputAuthority)
        {
            InputReader.playerControllerInputBlocked = false;
        }
    }

    public void OnAnimationFinished()
    {
        if (_subFSM.ActiveState is IAnimationActionEndNotify notify)
        {
            Debug.Log("Berserk State: OnAnimationFinished Called");
            notify.OnAnimationFinished();
        }
    }
}
