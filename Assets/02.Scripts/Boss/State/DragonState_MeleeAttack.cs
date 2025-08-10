using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MeleeAttack : DragonStateBase, IParentState, IAnimationActionNotify, IAnimationExitActionNotify
{
    private StateMachine<DragonSubStateBase> _phase1SubStateMachine;
    private StateMachine<DragonSubStateBase> _phase2SubStateMachine;
    private StateMachine<DragonSubStateBase> _currentSubStateMachine;
    
    private DragonStateParameterSet.AttackParams _attackParams;

    public DragonState_MeleeAttack(DragonContext context)
        : base(context)
    {
        _attackParams = Context.Parameter.Attack;
    }

    protected override void OnEnterState()
    {
        //_currentSubStateMachine = Context.Phase.CurrentPhase == EDragonPhase.Phase1 ? _phase1SubStateMachine : _phase2SubStateMachine;
        _currentSubStateMachine = _phase2SubStateMachine;

        TryActiveRandomAttackSubState();
    }

    private void TryActiveRandomAttackSubState()
    {
        // 0번은 Prepare라고 약속, 하드코딩이지만 일단 진행
        int random = 2;// Random.Range(1, _currentSubStateMachine.States.Length);
        var nextState = _currentSubStateMachine.States[random];
        _currentSubStateMachine.TryActivateState(nextState, true);
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _phase1SubStateMachine = new StateMachine<DragonSubStateBase>("MeleeAttackSubFSM",
            new DragonMeleeAttack_Prepare(Context, this),
            new DragonMeleeAttack(Context, this, "Attack_Bite", Context.Parameter.Bite),
            new DragonMeleeAttack(Context, this, "Attack_LeftScratch", Context.Parameter.LeftScratch),
            new DragonMeleeAttack(Context, this, "Attack_RightScratch", Context.Parameter.RightScratch),
            new DragonMeleeAttack(Context, this, "Attack_Swipe", Context.Parameter.Swipe)
        );

        _phase2SubStateMachine = new StateMachine<DragonSubStateBase>("MeleeAttackSubFSM",
            new DragonMeleeAttack_Prepare(Context, this),
            new DragonMeleeAttack(Context, this, "Attack_LeftScratch", Context.Parameter.LeftScratch
                , () => Context.Combat.DarkProjectileEffect()),
            new DragonMeleeAttack(Context, this, "Attack_RightScratch", Context.Parameter.LeftScratch
                , () => Context.Combat.WindStormEffect())
        );

        stateMachines.Add(_phase1SubStateMachine);
        stateMachines.Add(_phase2SubStateMachine);
    }

    public void OnSubStateComplete()
    {
        bool inSight = Context.Sight.SightDetector.DetectedColliders.Count > 0;
        float distance = Context.Sight.Distance;
        // 시야에 있고 사거리 안이고 연속 공격 확률에 성공이면 공격
        float continueAttackRandom = Random.Range(0f, 1f);
        if (inSight &&
            distance < Context.Parameter.Base.MeleeAttackDistance &&
            continueAttackRandom < _attackParams.ContinueAttackChance)
        {
            TryActiveRandomAttackSubState();
            return;
        }

        // 너무 가까우면 후진
        if (_currentSubStateMachine.TryActivateState<DragonMeleeAttack_Prepare>(true))
        {
            return;
        }

        // 모두 아닌경우 다시 경계 태세
        Machine.TryActivateState<DragonState_Alert>(true);
    }

    public void OnActionMoment()
    {
        if (_currentSubStateMachine.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }

    public void OnExitMoment()
    {
        if (_currentSubStateMachine.ActiveState is IAnimationExitActionNotify notify)
        {
            notify.OnExitMoment();
        }
    }
}