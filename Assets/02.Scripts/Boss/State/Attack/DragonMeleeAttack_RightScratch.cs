public class DragonMeleeAttack_RightScratch : DragonSubStateBase
{
    private DragonStateParameterSet.RightScratchParams RightScratch;
    private bool _hasAttacked;

    public DragonMeleeAttack_RightScratch(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        RightScratch = Context.Parameter.RightScratch;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
        Context.Movement.ResetNavMeshAgent();
        
        Context.Movement.Lock();
        
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_RightScratch");
    }

    protected override void OnFixedUpdate()
    {
        if (Context.Movement.IsLocked)
        {
            return;
        }
        
        ParentState.OnSubStateComplete();
    }
}