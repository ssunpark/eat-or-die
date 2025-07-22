public class EnemyIdleState : IEnemyState<EnemyStateMachine>
{
    public bool IsInterruptable => true;

    public void Enter(EnemyStateMachine stateMachine)
    {
    }

    public void Update(EnemyStateMachine stateMachine, float deltaTime)
    {
    }

    public void Exit(EnemyStateMachine stateMachine)
    {
    }
}