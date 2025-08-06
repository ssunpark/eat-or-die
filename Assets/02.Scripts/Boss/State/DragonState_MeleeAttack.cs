using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MeleeAttack : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.AttackParams _attackParams;

    public DragonState_MeleeAttack(DragonContext context)
        : base(context)
    {
        _attackParams = Context.Parameter.Attack;
    }

    protected override void OnEnterState()
    {
        TryActiveRandomAttackSubState();
    }

    private void TryActiveRandomAttackSubState()
    {
        int random = Random.Range(0, 4); // 0~3

        switch (random)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonMeleeAttack_RightScratch>(true);
                break;
            case 2:
                _subStateMachine.TryActivateState<DragonMeleeAttack_LeftScratch>(true);
                break;
            case 3:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Bite>(true);
                break;
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("MeleeAttackSubFSM",
            new DragonMeleeAttack_Prepare(Context, this),
            new DragonMeleeAttack_Swipe(Context, this),
            new DragonMeleeAttack_RightScratch(Context, this),
            new DragonMeleeAttack_LeftScratch(Context, this),
            new DragonMeleeAttack_Bite(Context, this)
        );

        stateMachines.Add(_subStateMachine);
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
        if (_subStateMachine.TryActivateState<DragonMeleeAttack_Prepare>(true))
        {
            return;
        }

        // 모두 아닌경우 다시 경계 태세
        Machine.TryActivateState<DragonState_Alert>(true);
    }
}