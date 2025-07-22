using UnityEngine;
using Fusion;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private NetworkPrefabRef _enemyPrefab;
    
    public float SpawnDuration = 5f; // Spawn 간격
    private float _spawnTimer = 0f;

    private void Update()
    {
        if (Room.Instance == null || !Room.Instance.Runner.IsServer) return;
        
        if (_spawnTimer >= SpawnDuration)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
        }
        _spawnTimer += Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        Room.Instance.Runner.Spawn(_enemyPrefab, transform.position, Quaternion.identity);
    }
}
