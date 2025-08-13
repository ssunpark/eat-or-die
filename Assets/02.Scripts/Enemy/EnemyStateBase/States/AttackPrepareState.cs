using Fusion.Addons.FSM;

public class AttackPrepareState : AEnemyState
{
    protected override void OnEnterState()
    {
        Context.Owner.AnimationState = EAnimationState.AttackPrepare;
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= Context.StatManager.GetStat(EStatType.EnemyAttackSpeed))
        {
            Machine.TryActivateState<AttackActionState>();
        }
    }
}