using Fusion;
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

        var total = _breathParams.FireTime + _breathParams.Duration;
        Context.Combat.BreathTimer = TickTimer.CreateFromSeconds(Machine.Runner, total);
    }

    protected override void OnFixedUpdate()
    {
        if (!Context.Movement.IsLocked)
            ParentState.OnSubStateComplete();
    }

    protected override void OnEnterStateRender()
    {
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetBool("Attack_Breath", true);
    }

    protected override void OnRender()
    {
        if (Context.Combat.BreathTimer.Expired(Machine.Runner))
        {
            Context.Animator.SetBool("Attack_Breath", false);
        }
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