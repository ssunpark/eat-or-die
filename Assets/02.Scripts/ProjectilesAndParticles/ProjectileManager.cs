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
    }

    [SerializeField] private List<ProjectileEntry> _projectiles;

    private Dictionary<string, GameObject> _projectileMap = new();

    private void Awake()
    {
        foreach (var entry in _projectiles)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Key) && entry.Prefab != null)
            {
                _projectileMap[entry.Key] = entry.Prefab;
            }

        }
        ParticleManager.Instance.Init(_projectiles);

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

}
