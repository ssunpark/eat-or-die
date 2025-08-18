using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
public struct PlayerInfo : INetworkStruct
{
    public PlayerRef Ref;
    public NetworkString<_16> Nickname;
    public NetworkId NetworkId;
}
public class PlayerInfoManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static PlayerInfoManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _playerControllers ??= new Dictionary<PlayerRef, Player>();
        _destroyToken = this.GetCancellationTokenOnDestroy();
        DontDestroyOnLoad(gameObject);

    }

    public override void Spawned()
    {
        base.Spawned();
        _networkReady = true;
    }
    private CancellationToken _destroyToken;

    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private static Dictionary<PlayerRef, Player> _playerControllers = new();
    public static IDictionary<PlayerRef, Player> PlayerControllers => _playerControllers;
    
    public Player LocalPlayer => _playerControllers.FirstOrDefault(pair => pair.Value.HasInputAuthority).Value;

    public event Action<PlayerRef, int> OnPlayerUnregistered;

    [Networked, Capacity(8)]
    public NetworkArray<PlayerInfo> Players => default;
    private readonly Dictionary<PlayerRef, string> _pendingNicknames = new();
    private readonly Queue<(PlayerRef playerRef, string nickname, NetworkId netId)> _registerQueue = new();
    private bool _isProcessingRegister;
    private readonly Queue<PlayerRef> _pendingJoins = new();   // 씬 로드 끝날 때까지 대기

    private bool _networkReady;
    private bool _sceneReady;


    public Player GetPlayerFromNetworkId(NetworkId networkId)
    {
        var obj = Runner.FindObject(networkId);

        if (obj == null)
        {
            Debug.LogWarning($"No object found with NetworkId: {networkId}");
            return null;
        }
        if (obj.TryGetComponent(out Player player))
        {
            return player;
        }
        return null;
    }
    public void UnregisterPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer == false)
        {
            return;
        }
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Ref == playerRef)
            {
                Debug.Log($"Unregistering player: {playerRef} at index {i}");
                Players.Set(i, default);
                Rpc_InvokeUnregistered(playerRef, i);
                break;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_InvokeUnregistered(PlayerRef playerRef, int index, RpcInfo info = default)
    {
        OnPlayerUnregistered?.Invoke(playerRef, index);
    }
    public string GetNickname(PlayerRef playerRef)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Ref == playerRef)
                return Players[i].Nickname.ToString();
        }
        return "Unknown";
    }

    

    public void SetRunner(NetworkRunner runner)
    {
        runner.AddCallbacks(this);
    }

    private void EnqueueRegister(NetworkRunner runner, PlayerRef playerRef, string nickname, NetworkId netId)
    {
        if (!runner.IsServer) return;
        _registerQueue.Enqueue((playerRef, nickname, netId));
        if (!_isProcessingRegister)
            ProcessRegisterQueueAsync(runner).Forget();
    }

    private async UniTaskVoid ProcessRegisterQueueAsync(NetworkRunner runner)
    {
        var token = _destroyToken; // Awake에서 캐시된 토큰

        try
        {
            // 씬 준비 & 네트워크 attach 둘 다 기다림
            await UniTask.WaitUntil(() => _sceneReady && _networkReady && runner.IsServer,
                                    cancellationToken: token)
                         .Timeout(TimeSpan.FromSeconds(15)) // 필요시 늘리거나 제거
                         .SuppressCancellationThrow();

            while (!token.IsCancellationRequested && _registerQueue.Count > 0)
            {
                var (pref, nick, netId) = _registerQueue.Dequeue();
                RegisterPlayerInternal(pref, nick, netId); // 여기서 Players 접근
            }
        }
        catch (OperationCanceledException) { /* 정상 취소 */ }
        catch (TimeoutException)
        {
            Debug.LogWarning("[PIM] Queue processing timeout. Flags => " +
                             $"sceneReady:{_sceneReady}, networkReady:{_networkReady}, isServer:{runner.IsServer}");
            // 실패 시: 큐를 유지하고 다음 OnSceneLoadDone에서 다시 시도하도록 그냥 반환
        }
        finally
        {
            _isProcessingRegister = false;
        }
    }


    // Players 준비 확인 (네트워크 컨테이너 특성에 맞게 구현)
    private bool IsPlayersReady()
    {
        try
        {
            var len = Players.Length; // 여기에 접근해도 터지지 않으면 OK
            return len > 0 || len == 0; // 접근 자체가 성공하면 준비된 것
        }
        catch
        {
            return false;
        }
    }

    // 실제 등록 로직: 기존 RegisterPlayer 본문을 옮김
    private void RegisterPlayerInternal(PlayerRef playerRef, string nickname, NetworkId networkId)
    {
        // Players에 빈 슬롯을 찾아 셋팅
        for (int i = 0; i < Players.Length; i++)
        {
            if (!Players[i].Ref.IsRealPlayer)
            {
                Players.Set(i, new PlayerInfo
                {
                    Ref = playerRef,
                    Nickname = nickname,
                    NetworkId = networkId
                });
                break;
            }
        }

        // 대기중 닉네임 있으면 덮어쓰기
        if (_pendingNicknames.TryGetValue(playerRef, out var pending))
        {
            UpdateNickname(playerRef, pending);
            _pendingNicknames.Remove(playerRef);
        }
    }

    // 기존 UpdateNickname은 NetworkId 보존해서 덮어쓰기 (이미 적용했다면 그대로 유지)
    public void UpdateNickname(PlayerRef playerRef, string nickname)
    {
        if (Object == null || !Object.HasStateAuthority) return;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Ref == playerRef)
            {
                var prev = Players[i];
                Players.Set(i, new PlayerInfo
                {
                    Ref = playerRef,
                    Nickname = nickname,
                    NetworkId = prev.NetworkId // 보존 중요
                });
                return;
            }
        }

        // 아직 슬롯 없으면 대기
        _pendingNicknames[playerRef] = nickname;
    }

    private void TryProcessPendingJoins(NetworkRunner runner)
    {
        if (!runner.IsServer) return;
        if (!_sceneReady) return;                      // 씬 준비 전엔 절대 스폰 금지

        while (_pendingJoins.Count > 0)
        {
            var player = _pendingJoins.Dequeue();
            SafeSpawnPlayer(runner, player);
        }
    }

    private void SafeSpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        // 실제 스폰
        var spawnPos = new Vector3(0, 1, 0);
        var no = runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player);
        runner.SetPlayerObject(player, no);

        _playerControllers[player] = no.GetComponent<Player>();
        var customizeHandler = no.GetComponent<PlayerCustomizeHandler>();
        var networkObj = no.GetComponent<NetworkObject>();

        // 닉네임은 아직 기본값일 수 있음 → 일단 큐에 넣고, 나중에 커스터마이즈 RPC가 오면 _pendingNicknames로 덮어씌움
        var nickname = customizeHandler != null && !string.IsNullOrEmpty(customizeHandler.Nickname)
            ? customizeHandler.Nickname
            : $"Player{UnityEngine.Random.Range(100, 999)}";

        EnqueueRegister(runner, player, nickname, networkObj.Id);
        // 이후 Register/닉네임 큐잉/대기열 처리 등 기존 로직 호출
        // EnqueueRegister(runner, player, nickname, no.Id);  // Runner 인자 전달 방식 권장
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;
        _pendingJoins.Enqueue(player);
        TryProcessPendingJoins(runner);
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            UnregisterPlayer(runner, player);

            if (!_playerControllers.TryGetValue(player, out Player playerController))
            {
                Debug.LogWarning($"[OnPlayerLeft] {player} 컨트롤러 없음.");
                return;
            }

            if (!playerController.NetworkObject.IsValid)
            {
                Debug.LogWarning($"[OnPlayerLeft] {player}의 NetworkObject는 Spawn되지 않았음.");
                return;
            }

            Debug.Log($"[OnPlayerLeft] {player} Despawn 시도.");
            _playerControllers.Remove(player);
            runner.Despawn(playerController.Object);
        }
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
}

void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
}

void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
}

void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
}

void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
}

void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
}

void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
}

void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
}

void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
}

void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
}

void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List < SessionInfo > sessionList)
    {
}

void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
}

void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
}

void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
        _sceneReady = true; 
        TryProcessPendingJoins(runner);
    }

void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
        _sceneReady = false;
    }
}