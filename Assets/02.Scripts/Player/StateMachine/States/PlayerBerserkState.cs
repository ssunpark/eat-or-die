using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
public class PlayerBerserkState : APlayerStateBase, IAnimationActionNotify
{
    private StateMachine<ABerserkSubStateBase> _subFSM;
    private readonly BerserkChase _chase;
    private readonly BerserkAttack _attack;

    public PlayerBerserkState(PlayerFSM fsm) : base(fsm)
    {
        _chase = new BerserkChase(fsm);
        _attack = new BerserkAttack(fsm);
        _subFSM = new StateMachine<ABerserkSubStateBase>("Berserk FSM",
            new BerserkIdle(fsm), // 초기 상태
            _chase,
            _attack
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
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        KCC.Move(Vector3.zero); // 이동 멈춤
        Debug.Log("PlayerBerserkState: Entering Berserk state.");
        _subFSM.Reset(); // 내부 상태머신 초기화
        ParticleManager.Instance.PlayByKey(_fsm.HungryEffect.name, _fsm.transform.position, _fsm.transform.rotation, true);
    }
    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
    }

    protected override void OnFixedUpdateState()
    {
        _resource.ConsumeHunger(Machine.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime) * 2);
    }

    public void OnActionMoment()
    {
        _subFSM.ActiveState?.OnActionMoment();
    }
}
