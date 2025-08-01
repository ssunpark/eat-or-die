public class DragonMeleeAttack_LeftScratch : DragonSubStateBase
{
    private DragonStateParameterSet.LeftScratchParams _leftScratchParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_LeftScratch(
        DragonController controller,
        IParentState parent,
        DragonStateParameterSet.LeftScratchParams leftScratch)
        : base(controller, parent)
    {
        _leftScratchParams = leftScratch;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
        Controller.NavMeshAgent.ResetPath();
        
        Controller.Lock();
        
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetTrigger("Attack_LeftScratch");
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