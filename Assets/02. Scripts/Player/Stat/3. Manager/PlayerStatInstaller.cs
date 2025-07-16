using UnityEngine;

public class PlayerStatInstaller : MonoBehaviour
{
    private PlayerStatManager _statManager;

    public PlayerStatManager StatManager => _statManager;

    [SerializeField] private bool useMockData = true;

    private void Awake()
    {
        // 1. Repository 준비
        IStatDataRepository repo = useMockData
            ? new MockStatDataRepository()
            : new StatDataRepository();

        // 2. 도메인 서비스 생성
        _statManager = new PlayerStatManager(repo);

        // 3. 네트워크 동기화 초기화
        var networkSync = GetComponent<PlayerStatNetworkSync>();
        if (networkSync != null)
            networkSync.Initialize(_statManager);

        // 4. UI 등 연결
        var debugger = GetComponent<PlayerStatDebugger>();
        if (debugger != null)
            debugger.Bind(_statManager);
    }
}
