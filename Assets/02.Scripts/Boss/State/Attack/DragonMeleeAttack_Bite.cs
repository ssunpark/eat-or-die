public class DragonMeleeAttack_Bite : DragonSubStateBase
{
    private DragonStateParameterSet.BiteParams _biteParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_Bite(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        _biteParams = Context.Parameter.Bite;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;

        Context.Movement.ResetNavMeshAgent();
        
        Context.Movement.Lock();
        
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_Bite");
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