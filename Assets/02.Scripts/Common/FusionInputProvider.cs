using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionInputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    private InputReader _inputReader;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner _runner;

    private Dictionary<EStatType, float> _statInputs = new();

    private void Awake()
    {
        _inputReader = FindAnyObjectByType<InputReader>();

    }

    public void SetRunner(NetworkRunner runner)
    {
        _runner = runner;
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_inputReader == null) return;

        var move = _inputReader.MoveInput;
        var data = new NetworkInputData
        {
            direction = new Vector3(move.x, 0, move.y),
            isAttacking = _inputReader.ConsumeAttackInput(),
            isRunning = _inputReader.IsRunning,
            isJumping = _inputReader.ConsumeJumpInput(),
            isInteracting = _inputReader.ConsumeInteractionInput(),
            isUsing = _inputReader.ConsumeUseItemInput()
        };
        input.Set(data);
    }

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var baseStats = new Dictionary<EStatType, float>(_statInputs);

            // Vector3 spawnPos = new((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            Vector3 spawnPos = new Vector3(30, 0, 171); // DemoScene Spawn Position
            runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player);
        }
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }
}