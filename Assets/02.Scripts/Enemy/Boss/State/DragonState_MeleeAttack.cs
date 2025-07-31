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
        int rand = Random.Range(0, 4); // 0~3

        switch (rand)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonMeleeAttack_Swipe>(true);
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
            new DragonMeleeAttack_Swipe(Controller, this, ParameterLoader.Swipe)
            // new DragonAttack_Swipe(Controller, this),
            // new DragonAttack_Swipe(Controller, this),
            // new DragonAttack_Swipe(Controller, this)
        );

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        float prepareChance = Random.Range(0f, 1f);
        // 너무 가까우면 후진
        if (Vector3.Distance(
                Controller.transform.position,
                Controller.Target.transform.position
            ) < ParameterLoader.Prepare.MinDistanceToFinishPrepare &&
            prepareChance < _attackParams.PrepareChance)
        {
            _subStateMachine.TryActivateState<DragonMeleeAttack_Prepare>(true);
            return;
        }

        // 이후 둘중 하나
        float continueAttackChance = Random.Range(0f, 1f);
        if (continueAttackChance < _attackParams.ContinueAttackChance)
        {
            TryActiveRandomAttackSubState();
            return;
        }
        Machine.TryActivateState<DragonState_Alert>(true);
    }
}