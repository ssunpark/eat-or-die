public class DragonMeleeAttack_RightScratch : DragonSubStateBase
{
    private DragonStateParameterSet.RightScratchParams RightScratch;
    private bool _hasAttacked;

    public DragonMeleeAttack_RightScratch(
        DragonController controller,
        IParentState parent,
        DragonStateParameterSet.RightScratchParams rightScratch)
        : base(controller, parent)
    {
        RightScratch = rightScratch;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
        Controller.NavMeshAgent.ResetPath();
        
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetTrigger("Attack_RightScratch");
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return;
        }
        
        ParentState.OnSubStateComplete();
    }
}