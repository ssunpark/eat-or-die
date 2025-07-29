using UnityEngine;
using Fusion.Addons.FSM;

public class DragonState_Wait : DragonSubStateBase
{
    private float _waitDuration = 3f;

    public DragonState_Wait(DragonController controller, IParentState parent) : base(controller, parent)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Idle 대기 상태 진입");


        Controller.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
        Controller.Animator.SetBool("IsMove", false);
    }

    protected override void OnFixedUpdate()
    {
        if (Controller.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (Machine.StateTime >= _waitDuration)
        {
            Debug.Log("대기 끝");
            Controller.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
            ParentState.OnSubStateComplete();
        }
    }
}