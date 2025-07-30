using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
public class PlayerBerserkState : APlayerStateBase
{
    private StateMachine<ABerserkSubStateBase> _subFSM;
    private readonly BerserkChase _chase;
    private readonly BerserkAttack _attack;

    public PlayerBerserkState(PlayerController controller) : base(controller)
    {
        _subFSM = new StateMachine<ABerserkSubStateBase>("BerserkFSM",
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
    protected override void OnEnterState()
    {
        _controller.Movement.Move(Vector3.zero, false); // 이동 멈춤

        _subFSM.Reset(); // 내부 상태머신 초기화
        if (_controller.Object.HasInputAuthority)
        {
            _controller.RPC_SetMoveFlag(true);
            _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Berserk);
        }
    }

    protected override void OnFixedUpdate()
    {
        _resource.ConsumeHunger(Machine.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime) * 5);
    }

    protected override void OnExitState()
    {
    }

}
