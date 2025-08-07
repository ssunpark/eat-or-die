public interface IEventReceiver
{
    // 해당 인터페이스를 구현하는 StateBehaviour는
    // OnEnterState와 OnExitState에서 EventReceiver를 설정하고 해제해야 합니다.
    public void OnActionMoment();
}