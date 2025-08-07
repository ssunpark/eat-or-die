using UnityEngine;

public class DragonMeleeAttack_Swipe : DragonSubStateBase, IAnimationActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.SwipeParams _swipeParams;
    private bool _hasAttacked;

    public DragonMeleeAttack_Swipe(
        DragonContext context,
        IParentState parent)
        : base(context, parent)
    {
        _swipeParams = Context.Parameter.Swipe;
    }

    protected override void OnEnterState()
    {
        _hasAttacked = false;

        Context.Movement.ResetNavMeshAgent();
        
        Context.Movement.Lock();

        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_Swipe");
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
        Context.Combat.SetDetector(_swipeParams.DetectRadius, _swipeParams.Angle);
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