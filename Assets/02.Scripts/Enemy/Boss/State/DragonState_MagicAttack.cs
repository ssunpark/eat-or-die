using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MagicAttack : DragonStateBase, IParentState
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.MagicParams _magicParams;

    public DragonState_MagicAttack(DragonController controller, DragonParameterLoader loader)
        : base(controller, loader)
    {
        _magicParams = ParameterLoader.Magic;
    }

    protected override void OnEnterState()
    {
        TryActivateRandomMagicSkill();
    }

    private void TryActivateRandomMagicSkill()
    {
        int rand = Random.Range(0, 3); // 확장 가능

        switch (rand)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonMagicAttack_Breath>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonMagicAttack_Lava>(true);
                break;
            case 2:
                _subStateMachine.TryActivateState<DragonMagicAttack_Roar>(true);
                break;
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("MagicAttackSubFSM",
            new DragonMagicAttack_Breath(Controller, this, ParameterLoader.Breath),
            new DragonMagicAttack_Lava(Controller, this, ParameterLoader.Lava),
            new DragonMagicAttack_Roar(Controller, this, ParameterLoader.Roar)
        );

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        float distance = Vector3.Distance(
            Controller.transform.position,
            Controller.Target.transform.position
        );

        bool inSight = Controller.SightDetector.DetectedColliders.Count > 0;
        float rand = Random.value;

        if (inSight && rand < _magicParams.ContinueMagicProbability)
        {
            TryActivateRandomMagicSkill();
            return;
        }

        Machine.TryActivateState<DragonState_Alert>(true);
    }
}