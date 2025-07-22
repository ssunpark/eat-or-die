public interface IEnemyState<T>
{
    public bool IsInterruptable { get; }
    public void Enter(T stateMachine);
    public void Update(T stateMachine, float deltaTime);
    public void Exit(T stateMachine);
}