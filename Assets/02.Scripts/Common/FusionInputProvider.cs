using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionInputProvider : SimulationBehaviour, INetworkRunnerCallbacks
{
    private InputReader _inputReader;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private NetworkRunner _runner;

    private Dictionary<EStatType, float> _statInputs = new();
    [HideInInspector]public Vector3[] SpawnPoint;

    public enum SpawnPosition
    {
        DemoScene,
        Origin
    } 
    public SpawnPosition SpawnPos;
    private void Awake()
    {
        _inputReader = FindAnyObjectByType<InputReader>();

        SpawnPoint = new Vector3[2];
        SpawnPoint[(int)SpawnPosition.DemoScene] = new Vector3(30, 0, 171);
        SpawnPoint[(int)SpawnPosition.Origin] = new Vector3(0, 1, 0);
    }

    public void SetRunner(NetworkRunner runner)
    {
        _runner = runner;
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);
    }


    private NetworkButtons _prevButtons;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_inputReader == null) return;

        var move = _inputReader.MoveInput;
        var currentButtons = new NetworkButtons();

        currentButtons.Set(EButtons.Attack, _inputReader.InputActions.Player.Attack.IsPressed());
        currentButtons.Set(EButtons.Interact, _inputReader.InputActions.Player.Interact.IsPressed());
        currentButtons.Set(EButtons.UseItem, _inputReader.InputActions.Player.UseItem.IsPressed());
        currentButtons.Set(EButtons.Run, _inputReader.InputActions.Player.Sprint.IsPressed());

        var data = new NetworkInputData
        {
            direction = new Vector2(move.x, move.y),
            buttons = currentButtons,
            previousButtons = _prevButtons
        };

        _prevButtons = currentButtons;
        input.Set(data);
    }

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var baseStats = new Dictionary<EStatType, float>(_statInputs);

            Vector3 spawnPos = SpawnPoint[(int)SpawnPos];
            //new((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0)
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
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
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

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }
}