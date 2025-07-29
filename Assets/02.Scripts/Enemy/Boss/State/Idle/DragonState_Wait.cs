using UnityEngine;
using Fusion.Addons.FSM;

public class DragonState_Wait : DragonSubStateBase
{
    private float _waitDuration = 3f;

    public DragonState_Wait(DragonStateMachine machine, IParentStateMachine parentMachine) : base(machine, parentMachine)
    {
    }

    protected override void OnEnterState()
    {
        Debug.Log("Idle 대기 상태 진입");


        StateMachine.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
        StateMachine.Animator.SetBool("IsMove", false);
    }

    protected override void OnFixedUpdate()
    {
        if (StateMachine.IsLocked)
        {
            return; // 잠금 상태면 아무 것도 안 함
        }

        if (Machine.StateTime >= _waitDuration)
        {
            Debug.Log("대기 끝");
            StateMachine.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
            ParentStateMachine.OnSubStateComplete();
        }
    }
}