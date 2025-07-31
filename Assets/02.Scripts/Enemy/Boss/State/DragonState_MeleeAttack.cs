using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MeleeAttack : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.AttackParams _attackParams;

    public DragonState_MeleeAttack(DragonController controller, DragonParameterLoader loader)
        : base(controller, loader)
    {
        _attackParams = ParameterLoader.Attack;
    }

    protected override void OnEnterState()
    {
        TryActiveRandomAttackSubState();
    }

    private void TryActiveRandomAttackSubState()
    {
        int random = Random.Range(0, 2); // 0~3

        switch (random)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonMeleeAttack_RightScratch>(true);
                break;
            case 2:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
                break;
            case 3:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
                break;
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("MeleeAttackSubFSM",
            new DragonMeleeAttack_Prepare(Controller, this, ParameterLoader.Prepare),
            new DragonMeleeAttack_Swipe(Controller, this, ParameterLoader.Swipe),
            new DragonMeleeAttack_RightScratch(Controller, this, ParameterLoader.RightScratch)
            // new DragonAttack_Swipe(Controller, this),
            // new DragonAttack_Swipe(Controller, this)
        );

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        bool inSight = Controller.SightDetector.DetectedColliders.Count > 0;
        float distance = Vector3.Distance(
            Controller.transform.position,
            Controller.Target.transform.position
        );
        // 시야에 있고 사거리 안이고 연속 공격 확률에 성공이면 공격
        float continueAttackRandom = Random.Range(0f, 1f);
        if (inSight &&
            distance < ParameterLoader.Base.MeleeAttackDistance &&
            continueAttackRandom < _attackParams.ContinueAttackChance)
        {
            TryActiveRandomAttackSubState();
            return;
        }

        // 시야 밖인 경우 대기 혹은 경계

        float prepareRandom = Random.Range(0f, 1f);

        // 너무 가까우면 후진
        if (distance < ParameterLoader.Prepare.MinDistanceToFinishPrepare &&
            prepareRandom < _attackParams.PrepareChance)
        {
            _subStateMachine.TryActivateState<DragonMeleeAttack_Prepare>(true);
            return;
        }

        // 모두 아닌경우 다시 경계 태세
        Machine.TryActivateState<DragonState_Alert>(true);
    }
}