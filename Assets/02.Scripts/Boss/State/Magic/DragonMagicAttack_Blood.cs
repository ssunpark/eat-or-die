using UnityEngine;

public class DragonMagicAttack_Blood : DragonSubStateBase, IAnimationEntryActionNotify, IAnimationExitActionNotify
{
    private DragonStateParameterSet.BloodParams _bloodParams;

    private int _spawnCount = 0;

    public DragonMagicAttack_Blood(
        DragonContext context,
        IParentState parentState)
        : base(context, parentState)
    {
        _bloodParams = Context.Parameter.Blood;
    }

    protected override void OnEnterState()
    {
        Context.Movement.Lock();
    }

    protected override void OnFixedUpdate()
    {
        if (!Context.Movement.IsLocked && 
            Machine.StateTime > _bloodParams.ChargeDuration)
        {
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnEnterStateRender()
    {
        _spawnCount = 0;
        Context.Animator.SetBool("IsMove", false);
        Context.Animator.SetTrigger("Attack_Blood");
    }

    public void OnEntryMoment()
    {
        Context.Combat.PerformBloodExplode(_bloodParams.ChargeDuration, _bloodParams.TargetSize);
    }

    public void OnExitMoment()
    {
        Context.Movement.Unlock();
    }
}