using UnityEngine;
using Fusion.Addons.FSM;

public class DragonState_Wait : DragonSubStateBase
{
    private DragonStateParameterSet.WaitParams _waitParams;

    public DragonState_Wait(DragonController controller, IParentState parent, DragonStateParameterSet.WaitParams waitParams) : base(controller, parent)
    {
        _waitParams = waitParams;
    }

    protected override void OnEnterState()
    {
        Controller.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
        Controller.Animator.SetBool("IsMove", false);
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (Machine.StateTime >= _waitParams.WaitDuration)
        {
            Controller.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
            ParentState.OnSubStateComplete();
        }
    }
}