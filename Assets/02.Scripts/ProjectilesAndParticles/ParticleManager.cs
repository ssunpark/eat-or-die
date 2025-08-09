using System;
using System.Collections.Generic;
using Fusion;
using Redcode.Pools;
using UnityEngine;
using static ProjectileManager;

public class ParticleManager: NetworkBehaviourSingleton<ParticleManager>
{
    private Dictionary<ParticleSystem, Pool<ParticleSystem>> _sharedPools = new();
    private Dictionary<string, ParticleSystem> _particlePrefabs = new();
    public IReadOnlyDictionary<ParticleSystem, Pool<ParticleSystem>> ExplodeEffectPool => _sharedPools;

    public void Init(IList<ProjectileEntry> projectiles)
    {
        foreach (var entry in projectiles)
        {
            if (entry.ExplodeEffectPrefab != null)
            {
                if(_particlePrefabs.ContainsKey(entry.ExplodeEffectPrefab.name))
                {
                    continue;
                }
                _particlePrefabs[entry.ExplodeEffectPrefab.name] = entry.ExplodeEffectPrefab;
                var (pool, parent) = GetOrCreateSharedPool(entry.ExplodeEffectPrefab, transform);
            }

        }
    }

    public void Init(IList<ParticleSystem> particleSystems)
    {
        foreach (var particleSystem in particleSystems)
        {
            if (_particlePrefabs.ContainsKey(particleSystem.name))
            {
                continue;
            }
            _particlePrefabs[particleSystem.name] = particleSystem;
            var (pool, parent) = GetOrCreateSharedPool(particleSystem, transform);
        }
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcPlayParticle(string particleKey, Vector3 position, Quaternion rotation)
    {
        // 개같은 RPC
        ParticleSystem particlePrefab = GetParticlePrefab(particleKey);
        
        if (particlePrefab != null)
        {
            PlayParticle(particlePrefab, position, rotation);
        }
        else
        {
            Debug.LogError($"Particle prefab with key '{particleKey}' not found.");
        }
    }

    private ParticleSystem GetParticlePrefab(string particleKey)
    {
        if (_particlePrefabs.TryGetValue(particleKey, out var prefab))
        {
            return prefab;
        }
        Debug.LogWarning($"Particle prefab with key '{particleKey}' not found.");
        return null;
    }

    public void PlayParticle(ParticleSystem particleSystem, Vector3 position, Quaternion rotation)
    {
        if (particleSystem == null)
        {
            Debug.LogWarning("Particle system is null. Cannot play particle.");
            return;
        }
        if (!_sharedPools.TryGetValue(particleSystem, out var pool))
        {
            _sharedPools[particleSystem] = pool = GetOrCreateSharedPool(particleSystem, transform).Item1;
        }
        ParticleSystem instance = pool.Get();
        instance.transform.position = position;
        instance.transform.rotation = rotation;

        var autoReturn = instance.GetComponent<ParticleAutoReturn>();
        autoReturn.Init(pool);

        instance.Play();

    }
}