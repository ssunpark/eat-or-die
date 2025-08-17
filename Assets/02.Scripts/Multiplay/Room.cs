using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : BehaviourSingleton<Room>, INetworkRunnerCallbacks
{
    public NetworkRunner RunnerPrefab;
    private NetworkRunner _runner;
    public NetworkRunner Runner => _runner;
    
    private GameObject _localPlayer;
    public GameObject LocalPlayer => _localPlayer;
    public event Action<NetworkRunner> OnGameStarted;
    public void SetLocalPlayer(GameObject player) => _localPlayer = player;

    private PlayerInfoManager _playerInfoManager;
    private FusionInputProvider _inputProvider;

    public void HostStart()
    {
        StartGame(GameMode.Host);
    }

    public void ClientStart()
    {
        StartGame(GameMode.Client);
    }

    public async void StartGame(GameMode mode)
    {
        _runner = Instantiate(RunnerPrefab);
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        var scene = SceneRef.FromIndex(3);
        NetworkSceneInfo sceneInfo = new();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }


        // Guid를 사용해 매번 고유한 세션 이름을 생성합니다.
        var sessionName = "123123";
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName, // 고유한 이름으로 변경
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        
        OnGameStarted?.Invoke(_runner);
    }


    // =========================================================
    // dev용
    [SerializeField] private NetworkPrefabRef _cheatProxyPrefab;
    // =========================================================
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // 이 콜백은 모든 클라이언트에서 호출되지만, RPC 호출은 호스트(서버)만 해야 함
        if (Runner.IsServer)
        {
            Debug.Log($"새로운 플레이어 {player} 입장. RoomInfo 동기화를 시작합니다.");

            // 1. 현재 방 정보를 DTO로 변환 후 JSON 문자열로 직렬화
            var roomInfo = RoomInfoManager.Instance.CurrentRoomInfo.ToNetworkDTO();
            var json = JsonUtility.ToJson(roomInfo);

            // 2. 새로 들어온 'player'를 타겟으로 하여 RPC를 호출함
            RoomInfoManager.Instance.RPC_SyncRoomInfoToNewPlayer(player, json);

            // =========================================================
            // dev용
            var proxy = runner.Spawn(_cheatProxyPrefab, Vector3.zero, Quaternion.identity, player);
            proxy.name = $"CheatExecutor_{player.RawEncoded}";
            // =========================================================


        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }
    
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server: {reason}");
        SceneManager.LoadScene(0);
    }
    
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"Connection failed: {reason}");
    }
    
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public async void OnSceneLoadDone(NetworkRunner runner)
    {
        OnGameStarted?.Invoke(runner);

        _inputProvider = FindAnyObjectByType<FusionInputProvider>();
        if (_inputProvider != null)
        {
            _inputProvider.SetRunner(_runner);
        }
        else
        {
            Debug.LogError("FusionInputProvider not found in the scene.");
        }

        _playerInfoManager = FindAnyObjectByType<PlayerInfoManager>();
        if (_playerInfoManager != null)
        {
            _playerInfoManager.SetRunner(_runner);
        }
        else
        {
            Debug.LogError("PlayerInfoManager not found in the scene.");
        }

        await ParticleManager.Instance.InitFromCsvAsync();
        
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ } 
}
