using UnityEngine;
using Fusion.Addons.FSM;

public class DragonState_Wait : DragonSubStateBase
{
    private DragonStateParameterSet.WaitParams _waitParams;

    public DragonState_Wait(DragonContext context, IParentState parent) : base(context, parent)
    {
        _waitParams = Context.Parameter.Wait;
    }

    protected override void OnEnterState()
    {
        Context.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
        Context.Animator.SetBool("IsMove", false);
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= _waitParams.WaitDuration)
        {
            Context.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
            ParentState.OnSubStateComplete();
        }
    }
}