using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
public class PlayerBerserkState : APlayerStateBase
{
    private StateMachine<ABerserkSubStateBase> _subFSM;
    private readonly BerserkChase _chase;
    private readonly BerserkAttack _attack;

    public PlayerBerserkState(PlayerFSM controller) : base(controller)
    {
        _subFSM = new StateMachine<ABerserkSubStateBase>("Berserk FSM",
            new BerserkIdle(controller), // 초기 상태
            new BerserkChase(controller),
            new BerserkAttack(controller)
        );
        _subFSM.SetDefaultState(0);
        AnimState = "Berserk";

    }
    protected override void CollectChildStateMachines(List<IStateMachine> list)
    {
        list.Add(_subFSM);
    }
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
            Debug.LogError("PlayerBerserkState: Stat or Resource is null. Cannot enter state.");
            return;
        }
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        KCC.Move(Vector3.zero); // 이동 멈춤

        _subFSM.Reset(); // 내부 상태머신 초기화

        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    protected override void OnFixedUpdate()
    {

        _resource.ConsumeHunger(Machine.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime) * 5);
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
