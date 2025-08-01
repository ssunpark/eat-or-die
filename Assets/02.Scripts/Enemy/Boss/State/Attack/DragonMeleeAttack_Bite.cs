public class DragonMeleeAttack_Bite : DragonSubStateBase
{
    private DragonStateParameterSet.BiteParams _biteParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_Bite(
        DragonController controller,
        IParentState parent,
        DragonStateParameterSet.BiteParams bite)
        : base(controller, parent)
    {
        _biteParams = bite;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;
        
        Controller.NavMeshAgent.ResetPath();
        
        Controller.Lock();
        
        Controller.Animator.SetBool("IsMove", false);
        Controller.Animator.SetTrigger("Attack_Bite");
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