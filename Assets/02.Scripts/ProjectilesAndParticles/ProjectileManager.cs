using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class ProjectileManager : BehaviourSingleton<ProjectileManager>
{

    [System.Serializable]
    public class ProjectileEntry
    {
        public string Key;
        public GameObject Prefab;
        public ParticleSystem ExplodeEffectPrefab;
    }

    [SerializeField] private List<ProjectileEntry> _projectiles;

    private Dictionary<string, GameObject> _projectileMap = new();
    private Dictionary<ParticleSystem, Pool<ParticleSystem>> _sharedPools = new();
    public IReadOnlyDictionary<ParticleSystem, Pool<ParticleSystem>> ExplodeEffectPool => _sharedPools;

    private void Awake()
    {
        foreach (var entry in _projectiles)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Key) && entry.Prefab != null)
            {
                _projectileMap[entry.Key] = entry.Prefab;
            }
            // ExplodeEffectPrefab이 null이 아닐 때만 GetOrCreateSharedPool 호출
            if (entry.ExplodeEffectPrefab != null)
            {
                var (pool, parent) = GetOrCreateSharedPool(entry.ExplodeEffectPrefab, transform);
            }

        }

    }

    public GameObject GetProjectile(string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        if (_projectileMap.TryGetValue(key, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[ProjectileManager] Projectile with key '{key}' not found.");
        return null;
    }

    private (Pool<ParticleSystem>, Transform) GetOrCreateSharedPool(ParticleSystem key, Transform poolParent)
    {
        if (_sharedPools.TryGetValue(key, out var existingPool))
            return (existingPool, existingPool.Container);

        GameObject parent = new(key.name);
        parent.transform.SetParent(poolParent);
        var newPool = Pool.Create(key, 0, parent.transform);
        _sharedPools[key] = newPool;
        return (newPool, parent.transform);
    }
}
