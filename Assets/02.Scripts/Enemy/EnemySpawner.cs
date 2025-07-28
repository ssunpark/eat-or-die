using UnityEngine;
using Fusion;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private NetworkPrefabRef _enemyPrefab;
    
    public float SpawnDuration = 5f; // Spawn 간격
    private float _spawnTimer = 0f;
    private int n = 0;

    private void Update()
    {
        if (Room.Instance == null || !Room.Instance.Runner.IsServer) return;
        
        if (_spawnTimer >= SpawnDuration && n < 2)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
            n++;
        }
        _spawnTimer += Room.Instance.Runner.DeltaTime;
    }

    private void SpawnEnemy()
    {
        Room.Instance.Runner.Spawn(_enemyPrefab, gameObject.transform.position, Quaternion.identity, Room.Instance.Runner.LocalPlayer);
    }
}
