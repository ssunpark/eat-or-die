using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;

public class FusionInputProvider : SimulationBehaviour, INetworkRunnerCallbacks
{

    public void SetRunner(NetworkRunner runner)
    {
        runner.ProvideInput = true;
        runner.AddCallbacks(this);
    }


    private NetworkButtons _prevButtons;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new();
        if (InputReader.Instance == null || !InputReader.Instance.HaveControl())
        {
            input.Set(data);
            return;
        }

        var move = InputReader.Instance.MoveInput;
        var currentButtons = new NetworkButtons();

        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        currentButtons.Set(EButtons.Attack, !isOverUI && InputReader.Instance.InputActions.Player.Attack.IsPressed());
        currentButtons.Set(EButtons.Interact, InputReader.Instance.InputActions.Player.Interact.IsPressed());
        currentButtons.Set(EButtons.UseItem, InputReader.Instance.InputActions.Player.UseItem.IsPressed());
        currentButtons.Set(EButtons.Run, InputReader.Instance.InputActions.Player.Sprint.IsPressed());


        data.mousePosition = InputReader.Instance.MousePosition;
        data.previousButtons = _prevButtons;
        data.direction = new Vector2(move.x, move.y);
        data.buttons = currentButtons;

        _prevButtons = currentButtons;
        input.Set(data);
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