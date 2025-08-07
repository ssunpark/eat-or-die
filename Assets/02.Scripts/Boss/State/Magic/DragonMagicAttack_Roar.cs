using DG.Tweening;

public class DragonMagicAttack_Roar : DragonSubStateBase, IAnimationEntryActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.RoarParams _roarParams;

    private Sequence effectsSequence;

    public DragonMagicAttack_Roar(
        DragonContext context,
        IParentState parentState)
        : base(context, parentState)
    {
        _roarParams = Context.Parameter.Roar;
    }

    protected override void OnEnterState()
    {
        Context.Movement.Lock();
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetBool("Attack_Roar", true);
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= _roarParams.FireTime + _roarParams.Duration)
        {
            Context.Animator.SetBool("Attack_Roar", false);
        }

        if (!Context.Movement.IsLocked)
        {
            ParentState.OnSubStateComplete();
        }
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }

    public void OnEntryMoment()
    {
        Context.Combat.PerformRoarAttack(
            _roarParams.Radius,
            _roarParams.Count,
            _roarParams.Duration
        );
    }
}