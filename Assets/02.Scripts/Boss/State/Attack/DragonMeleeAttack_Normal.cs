public class DragonMeleeAttack_Normal : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.NormalAttackParams _normalAttackParams;
    private string _animation;

    public DragonMeleeAttack_Normal(
        DragonContext context,
        IParentState parent,
        string animation,
        DragonStateParameterSet.NormalAttackParams normalAttackParams)
        : base(context, parent)
    {
        _animation = animation;
        _normalAttackParams = normalAttackParams;
    }

    protected override void OnEnterState()
    {
        Context.Movement.ResetNavMeshAgent();

        Context.Movement.Lock();

        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger(_animation);
    }

    protected override void OnFixedUpdate()
    {
        if (Context.Movement.IsLocked)
        {
            return;
        }

        ParentState.OnSubStateComplete();
    }

    protected override void OnEnterStateRender()
    {
        Context.Combat.SetDetector(_normalAttackParams.DetectRadius, _normalAttackParams.Angle);
    }

    public void OnActionMoment()
    {
        Context.Combat.Attack();
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}