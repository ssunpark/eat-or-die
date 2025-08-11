using System;

public class DragonMeleeAttack : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.NormalAttackParams _normalAttackParams;
    private string _animation;

    private Action _specialAttack;

    public DragonMeleeAttack(
        DragonContext context,
        IParentState parent,
        string animation,
        DragonStateParameterSet.NormalAttackParams normalAttackParams,
        Action SpecialAttack = null)
        : base(context, parent)
    {
        _animation = animation;
        _normalAttackParams = normalAttackParams;
        _specialAttack = SpecialAttack;
    }

    protected override void OnEnterState()
    {
        Context.Movement.ResetNavMeshAgent();

        Context.Movement.Lock();
        
        Context.Combat.SetDetector(_normalAttackParams.DetectRadius, _normalAttackParams.Angle);
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
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger(_animation);
    }

    public void OnActionMoment()
    {
        Context.Combat.Attack();
        _specialAttack?.Invoke();
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}