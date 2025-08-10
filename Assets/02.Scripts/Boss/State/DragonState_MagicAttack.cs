using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class DragonState_MagicAttack : DragonStateBase, IParentState, IAnimationEntryActionNotify, IAnimationExitActionNotify, IAnimationActionNotify
{
    private StateMachine<DragonSubStateBase> _subStateMachine;
    private DragonStateParameterSet.MagicParams _magicParams;

    public DragonState_MagicAttack(DragonContext context)
        : base(context)
    {
        _magicParams = Context.Parameter.Magic;
    }

    protected override void OnEnterState()
    {
        TryActivateRandomMagicSkill();
    }

    private void TryActivateRandomMagicSkill()
    {
        _subStateMachine.TryActivateState<DragonMagicAttack_Blood>(true);
        return;
        float randProbability = Random.value;
        if (Context.Sight.Distance < Context.Parameter.Base.MeleeAttackDistance
            && randProbability < Context.Parameter.Magic.NearMagicProbability)
        {
            _subStateMachine.TryActivateState<DragonMagicAttack_Roar>(true);
            return;
        }

        int rand = Random.Range(0, 2); // 확장 가능
        switch (rand)
        {
            case 0:
                _subStateMachine.TryActivateState<DragonMagicAttack_Breath>(true);
                break;
            case 1:
                _subStateMachine.TryActivateState<DragonMagicAttack_Lava>(true);
                break;
        }
    }

    protected override void CollectChildStateMachines(List<IStateMachine> stateMachines)
    {
        _subStateMachine = new StateMachine<DragonSubStateBase>("MagicAttackSubFSM",
            new DragonMagicAttack_Breath(Context, this),
            new DragonMagicAttack_Lava(Context, this),
            new DragonMagicAttack_Roar(Context, this),
            new DragonMagicAttack_Blood(Context, this)
        );

        stateMachines.Add(_subStateMachine);
    }

    public void OnSubStateComplete()
    {
        float distance = Context.Sight.Distance;

        bool inSight = Context.Sight.SightDetector.DetectedColliders.Count > 0;
        float rand = Random.value;

        if (inSight && rand < _magicParams.ContinueMagicProbability)
        {
            TryActivateRandomMagicSkill();
            return;
        }

        Machine.TryActivateState<DragonState_Alert>(true);
    }

    public void OnEntryMoment()
    {
        if (_subStateMachine.ActiveState is IAnimationEntryActionNotify notify)
        {
            notify.OnEntryMoment();
        }
    }

    public void OnExitMoment()
    {
        if (_subStateMachine.ActiveState is IAnimationExitActionNotify notify)
        {
            notify.OnExitMoment();
        }
    }

    public void OnActionMoment()
    {
        if (_subStateMachine.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }
}