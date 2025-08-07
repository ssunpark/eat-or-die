public class DragonMeleeAttack_Special_LeftScratch : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    public DragonMeleeAttack_Special_LeftScratch(DragonContext context, IParentState parent) : base(context, parent)
    {
    }
    
    protected override void OnEnterState()
    {
        Context.Movement.ResetNavMeshAgent();

        Context.Movement.Lock();

        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_LeftScratch");
    }

    protected override void OnFixedUpdate()
    {
        if (Context.Movement.IsLocked)
        {
            return;
        }

        ParentState.OnSubStateComplete();
    }

    public void OnActionMoment()
    {
        Context.Combat.Attack();
        // 투사체 발사
        Context.Combat.DarkProjectileEffect();
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}