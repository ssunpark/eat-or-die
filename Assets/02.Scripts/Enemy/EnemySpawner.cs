using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private NetworkPrefabRef _enemyPrefab;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private int _areaMask = NavMesh.AllAreas;
    public float SpawnDuration = 5f; // Spawn 간격
    private float _spawnTimer = 0f;
    
    private NetworkObject _enemyInstance;
    private int _autoRemaining = 1;

    

    private void Update()
    {
        if (Room.Instance == null || !Room.Instance.Runner.IsServer) return;

        if (_autoRemaining > 0)
        {
            _spawnTimer += Room.Instance.Runner.DeltaTime;
            if (_spawnTimer >= SpawnDuration)
            {
                _spawnTimer = 0f;
                _enemyInstance = SpawnOnce();
                _autoRemaining--;
            }
        }
        
        if (_autoRemaining == 0 && _enemyInstance == null)
        {
            _autoRemaining++;
        }
    }

    private void SpawnEnemy()
    {
        Room.Instance.Runner.Spawn(_enemyPrefab, gameObject.transform.position, Quaternion.identity, Room.Instance.Runner.LocalPlayer);
    }



























    public static readonly List<EnemySpawner> Instances = new();

    private void OnEnable() { Instances.Add(this); }
    private void OnDisable() { Instances.Remove(this); }
    public NetworkObject SpawnOnce()
    {
        var r = Room.Instance.Runner;
        NetworkObject enemy;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            enemy = r.Spawn(_enemyPrefab, hit.position, Quaternion.identity, r.LocalPlayer);
            r.transform.SetPositionAndRotation(hit.position, Quaternion.identity);
            Debug.Log($"[ObjectPoolManager] 스폰 위치: {hit.position} (NavMesh 보정)");
        }
        else
        {
            Debug.LogWarning($"[ObjectPoolManager] {gameObject.name} 위치에 NavMesh가 없습니다. 기본 위치로 스폰합니다.");
            enemy = r.Spawn(_enemyPrefab, transform.position, Quaternion.identity, r.LocalPlayer);
        }

            return enemy;
    }

    // ★ 즉시 N마리 스폰(버스트)
    public void SpawnBurst(int count)
    {
        var r = Room.Instance.Runner;
        for (int i = 0; i < count; i++)
            r.Spawn(_enemyPrefab, transform.position, Quaternion.identity, r.LocalPlayer);
    }

    // ★ 자동 스폰 시작/정지
    public void StartAuto(float intervalSeconds, int totalCount)
    {
        SpawnDuration = Mathf.Max(0.01f, intervalSeconds);
        _spawnTimer = 0f;
        _autoRemaining = Mathf.Max(0, totalCount);
    }
    public void StopAuto()
    {
        _autoRemaining = 0;
        _spawnTimer = 0f;
    }
    public void SpawnAt(Vector3 position, int count = 1)
    {
        var r = Room.Instance.Runner;
        for (int i = 0; i < count; i++)
            r.Spawn(_enemyPrefab, position, Quaternion.identity, r.LocalPlayer);
    }
}
