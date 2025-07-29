using UnityEngine;

public class DragonWaitState : IEnemyState<DragonStateMachine>
{
    private readonly IParentStateMachine _parent;
    private float _timer = 3f;

    public bool IsInterruptable => true;

    public DragonWaitState(IParentStateMachine parent)
    {
        _parent = parent;
    }

    public void Enter(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 대기 상태 진입");

        stateMachine.Animator.SetInteger("IdleIndex", Random.Range(0, 2));
        stateMachine.Animator.SetBool("IsMove", false);
    }

    public void Update(DragonStateMachine stateMachine, float dt)
    {
        _timer -= dt;

        if (_timer <= 0f)
        {
            _parent.OnSubStateComplete();
        }
    }

    public void Exit(DragonStateMachine stateMachine)
    {
        Debug.Log("Idle 대기 상태 종료");
    }
}