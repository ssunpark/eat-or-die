using DG.Tweening;

public class DragonMagicAttack_Roar : DragonSubStateBase
{
    private DragonStateParameterSet.RoarParams _roarParams;
    private bool _onFired;

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
        if (!_onFired && Machine.StateTime >= _roarParams.FireTime)
        {
            _onFired = true;

            Context.Combat.PerformRoarAttack(
                _roarParams.Radius,
                _roarParams.Count,
                _roarParams.Duration
            );

            return;
        }

        if (Machine.StateTime >= _roarParams.FireTime + _roarParams.Duration)
        {
            Context.Animator.SetBool("Attack_Roar", false);
        }

        if (!Context.Movement.IsLocked)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnExitState()
    {
        _onFired = false;
    }
}