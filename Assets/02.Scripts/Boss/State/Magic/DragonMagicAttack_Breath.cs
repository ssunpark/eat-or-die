using Redcode.Pools;
using UnityEngine;

public class DragonMagicAttack_Breath : DragonSubStateBase, IAnimationEntryActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.BreathParams _breathParams;

    public DragonMagicAttack_Breath(
        DragonContext context,
        IParentState parentState)
        : base(context, parentState)
    {
        _breathParams = Context.Parameter.Breath;
    }

    protected override void OnEnterState()
    {
        Context.Movement.Lock();
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetBool("Attack_Breath", true);
    }

    protected override void OnFixedUpdate()
    {
        float t = Machine.StateTime;

        if (t >= _breathParams.FireTime + _breathParams.Duration)
            Context.Animator.SetBool("Attack_Breath", false);

        if (!Context.Movement.IsLocked)
            ParentState.OnSubStateComplete();
    }
    
    public void OnEntryMoment()
    {
        Context.Combat.PlayBreath(_breathParams.Duration);
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}