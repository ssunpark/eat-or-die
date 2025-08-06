public class DragonMeleeAttack_LeftScratch : DragonSubStateBase
{
    private DragonStateParameterSet.LeftScratchParams _leftScratchParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_LeftScratch(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        _leftScratchParams = Context.Parameter.LeftScratch;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
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
}