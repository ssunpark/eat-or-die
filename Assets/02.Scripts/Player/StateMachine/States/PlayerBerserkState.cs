using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;
using UnityEngine.InputSystem.XR;
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
    protected override bool CanExitState(IState nextState)
    {
        return _resource.GetHungerPercent() > 0.1f || _resource.GetHungerPercent()==0f;
    }
    protected override void OnEnterState()
    {

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
