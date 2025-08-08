using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
public struct PlayerInfo : INetworkStruct
{
    public PlayerRef Ref;
    public NetworkString<_16> Nickname;
    public NetworkId NetworkId;
}
public class PlayerInfoManager : NetworkBehaviourSingleton<PlayerInfoManager>, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private static Dictionary<PlayerRef, Player> _playerControllers = new();
    public static IDictionary<PlayerRef, Player> PlayerControllers => _playerControllers;

    public event Action<PlayerRef, int> OnPlayerUnregistered;

    [Networked, Capacity(8)]
    public NetworkArray<PlayerInfo> Players => default;

    public void RegisterPlayer(PlayerRef playerRef, string nickname, NetworkId networkId)
    {
        if (Runner.IsServer == false)
        {
            return;
        }
        for (int i = 0; i < Players.Length; i++)
        {
            if (!Players[i].Ref.IsRealPlayer)
            {
                Players.Set(i, new PlayerInfo { Ref = playerRef, Nickname = nickname, NetworkId = networkId });
                break;
            }
        }
    }

    public Player GetPlayerFromNetworkId(NetworkId networkId)
    {
        var obj = Runner.FindObject(networkId);
        Debug.Log($"GetPlayerFromNetworkId: {networkId} -> {obj}");

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

    public void UpdateNickname(PlayerRef playerRef, string nickname)
    {
        if (!Runner.IsServer) return;

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i].Ref == playerRef)
            {
                Players.Set(i, new PlayerInfo { Ref = playerRef, Nickname = nickname });
                break;
            }
        }
    }

    public void SetRunner(NetworkRunner runner)
    {
        runner.AddCallbacks(this);
    }
    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPos = new(0, 1, 0);
            //new((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0)
            var playerObj = runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player);
            runner.SetPlayerObject(player, playerObj);
            _playerControllers[player] = playerObj.GetComponent<Player>();
            var customizeHandler = playerObj.GetComponent<PlayerCustomizeHandler>();
            var networkObj = playerObj.GetComponent<NetworkObject>();
            RegisterPlayer(player, customizeHandler.Nickname, networkObj.Id);
        }
    }
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("Player joined");
            SpawnPlayer(runner, player);
        }
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
}

void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
}
}