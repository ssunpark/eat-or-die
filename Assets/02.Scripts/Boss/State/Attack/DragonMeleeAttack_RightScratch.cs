public class DragonMeleeAttack_RightScratch : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.RightScratchParams _rightScratch;
    private bool _hasAttacked;

    public DragonMeleeAttack_RightScratch(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        _rightScratch = Context.Parameter.RightScratch;
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
    
    protected override void OnEnterStateRender()
    {
        Context.Combat.SetDetector(_rightScratch.DetectRadius, _rightScratch.Angle);
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