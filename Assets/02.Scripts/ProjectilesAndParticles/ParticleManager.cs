using System;
using System.Collections.Generic;
using Fusion;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ProjectileManager;
using System.Threading.Tasks;

public class ParticleManager: NetworkBehaviourSingleton<ParticleManager>
{
    private const string PARTICLE_CSV_PATH = "/ParticlesCSV/Particles.csv"; // StreamingAssets 기준
    private Dictionary<ParticleSystem, Pool<ParticleSystem>> _sharedPools = new();
    private Dictionary<string, ParticleSystem> _particlePrefabs = new();
    public IReadOnlyDictionary<ParticleSystem, Pool<ParticleSystem>> ExplodeEffectPool => _sharedPools;

    public async Task InitFromCsvAsync(string relativeCsvPath = PARTICLE_CSV_PATH)
    {
        var fullPath = $"{Application.streamingAssetsPath}{relativeCsvPath}";
        List<ParticleRawData> rows;
        try
        {
            rows = CSVLoader<ParticleRawData>.LoadCSV(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ParticleManager] CSV 로드 실패: {fullPath}\n{e}");
            return;
        }

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.ParticleKey) || string.IsNullOrWhiteSpace(row.AddressablePath))
                continue;

            // 이미 등록되어 있으면 스킵
            if (_particlePrefabs.ContainsKey(row.ParticleKey))
                continue;

            GameObject prefab = null;
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(row.AddressablePath);
                prefab = await handle.Task;
                if (prefab == null)
                {
                    Debug.LogError($"[ParticleManager] Addressables 로드 실패: {row.AddressablePath}");
                    continue;
                }
                // Addressables.Release(handle); // 풀의 프리팹 참조가 필요하므로 해제하지 않음
            }
            catch (Exception e)
            {
                Debug.LogError($"[ParticleManager] Addressables 로드 중 예외: {row.AddressablePath}\n{e}");
                continue;
            }

            var particleSystem = prefab.GetComponent<ParticleSystem>();
            _particlePrefabs[row.ParticleKey] = particleSystem;
            GetOrCreateSharedPool(particleSystem, transform);
        }

        Debug.Log($"[ParticleManager] CSV 기반 파티클 초기화 완료. Count={_particlePrefabs.Count}");
    }

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
            return prefab;

        foreach (var kv in _particlePrefabs)
            if (kv.Value != null && kv.Value.name == particleKey)
                return kv.Value;

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