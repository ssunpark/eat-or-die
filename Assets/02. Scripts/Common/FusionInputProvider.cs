using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionInputProvider : MonoBehaviour, INetworkRunnerCallbacks
{
    private InputReader _inputReader;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private NetworkRunner _runner;

    private string _nickname = "Player";
    private Dictionary<EStatType, float> _statInputs = new();
    private Dictionary<string, int> _customizeSelections = new();

    private void Awake()
    {
        _inputReader = FindAnyObjectByType<InputReader>();

        // 초기화
        foreach (EStatType stat in Enum.GetValues(typeof(EStatType)))
        {
            if (stat != EStatType.MaxHealth)
                _statInputs[stat] = 10f;
        }

        string[] categories = new string[] {
            "Axe", "Bag", "Bottom", "Bracelet", "Earring", "Eye", "Eyebrow", "Eyewear",
            "Glove", "Hair", "HairAcc", "HandAcc", "Headgear", "Lips", "Mask", "Mustache",
            "Shield", "Shoes", "Spear", "Sword", "Top", "Watch"
        };

        foreach (var category in categories)
            _customizeSelections[category] = 1;
    }

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "FuckyouFusion",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height));
        GUILayout.Label("Nickname:");
        _nickname = GUILayout.TextField(_nickname);

        GUILayout.Space(10);
        GUILayout.Label("Stats:");
        foreach (EStatType stat in Enum.GetValues(typeof(EStatType)))
        {
            if (stat == EStatType.MaxHealth) continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(stat.ToString(), GUILayout.Width(140));
            string input = GUILayout.TextField(_statInputs[stat].ToString("F2"), GUILayout.Width(60));
            input = input.Replace(',', '.');
            if (float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float val))
                _statInputs[stat] = val;
            GUILayout.EndHorizontal();
        }


        GUILayout.Space(10);
        GUILayout.Label("Customization:");
        Dictionary<string, int> maxCounts = new()
        {
            ["Axe"] = 3,
            ["Bag"] = 18,
            ["Bottom"] = 55,
            ["Bracelet"] = 5,
            ["Earring"] = 20,
            ["Eye"] = 12,
            ["Eyebrow"] = 23,
            ["Eyewear"] = 18,
            ["Glove"] = 22,
            ["Hair"] = 28,
            ["HairAcc"] = 3,
            ["HandAcc"] = 10,
            ["Headgear"] = 63,
            ["Lips"] = 11,
            ["Mask"] = 5,
            ["Mustache"] = 29,
            ["Shield"] = 4,
            ["Shoes"] = 52,
            ["Spear"] = 3,
            ["Sword"] = 3,
            ["Top"] = 71,
            ["Watch"] = 5
        };

        foreach (var kvp in maxCounts)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(kvp.Key, GUILayout.Width(100));
            string input = GUILayout.TextField(_customizeSelections[kvp.Key].ToString(), GUILayout.Width(50));
            if (int.TryParse(input, out int val))
                _customizeSelections[kvp.Key] = Mathf.Clamp(val, 0, kvp.Value); // 0 허용
            GUILayout.EndHorizontal();
        }

        

        if (_runner == null)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Host", GUILayout.Height(30))) StartGame(GameMode.Host);
            if (GUILayout.Button("Join", GUILayout.Height(30))) StartGame(GameMode.Client);
        }
        GUILayout.EndArea();
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
            isInteracting = _inputReader.ConsumeInteractionInput()
        };
        input.Set(data);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            var baseStats = new Dictionary<EStatType, float>(_statInputs);

            string GetName(string part) => _customizeSelections[part] > 0 ? part + "_" + _customizeSelections[part] : "";

            var spawnData = new PlayerSpawnData(
                CharacterClassType.Farmer, // 직업은 임시로 Farmer
                GetName("Axe"), GetName("Bag"), GetName("Bottom"), GetName("Bracelet"), GetName("Earring"),
                GetName("Eye"), GetName("Eyebrow"), GetName("Eyewear"), GetName("Glove"), GetName("Hair"),
                GetName("HairAcc"), GetName("HandAcc"), GetName("Headgear"), GetName("Lips"), GetName("Mask"),
                GetName("Mustache"), GetName("Shield"), GetName("Shoes"), GetName("Spear"), GetName("Sword"),
                GetName("Top"), GetName("Watch"), baseStats, _nickname);

            Vector3 spawnPos = new((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player,
                onBeforeSpawned: (runner, obj) =>
                {
                    var handler = obj.GetComponent<PlayerClassHandler>();
                    handler.SetCharacterInfo(
                        spawnData.classType, spawnData.Nickname,
                        spawnData.axe, spawnData.bag, spawnData.bottom, spawnData.bracelet, spawnData.earring,
                        spawnData.eye, spawnData.eyebrow, spawnData.eyewear, spawnData.glove,
                        spawnData.hair, spawnData.hairAcc, spawnData.handAcc, spawnData.headgear,
                        spawnData.lips, spawnData.mask, spawnData.mustache, spawnData.shield,
                        spawnData.shoes, spawnData.spear, spawnData.sword, spawnData.top, spawnData.watch);

                    var stat = obj.GetComponent<PlayerStat>();
                    stat.ApplyBaseStats(spawnData.baseStats);
                });
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out var networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
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
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }
}