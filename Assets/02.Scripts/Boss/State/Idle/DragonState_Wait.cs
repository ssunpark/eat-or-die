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
        Context.Animation.SetRandomWaitAnimation();
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime >= _waitParams.WaitDuration)
        {
            Context.Animation.SetRandomWaitAnimation();
            ParentState.OnSubStateComplete();
        }
    }

    protected override void OnEnterStateRender()
    {
        Context.Animator.SetBool("IsMove", false);
    }
}