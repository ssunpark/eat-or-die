using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MeleeAttack : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;

    public DragonState_MeleeAttack(DragonController controller, DragonParameterLoader loader)
        : base(controller, loader)
    {
    }

    protected override void OnEnterState()
    {
        TryActiveRandomSubState();
    }

    private void TryActiveRandomSubState()
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
            new DragonMeleeAttack_Swipe(Controller, this, ParameterLoader.Swipe)
            // new DragonAttack_Swipe(Controller, this),
            // new DragonAttack_Swipe(Controller, this),
            // new DragonAttack_Swipe(Controller, this)
        );

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        // 너무 가까우면 후진
        // 이후 둘중 하나
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                TryActiveRandomSubState();
                break;
            case 1:
                Machine.TryActivateState<DragonState_Alert>(true); // or DragonState_Fight
                break;
        }
    }
}