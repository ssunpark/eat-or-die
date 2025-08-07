public class DragonMeleeAttack_LeftScratch : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
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
    
    protected override void OnEnterStateRender()
    {
        Context.Combat.SetDetector(_leftScratchParams.DetectRadius, _leftScratchParams.Angle);
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