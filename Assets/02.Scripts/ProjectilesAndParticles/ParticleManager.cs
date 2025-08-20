using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DamageNumbersPro;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ParticleNetworkProxy;
using static ProjectileManager;

public class ParticleManager: BehaviourSingleton<ParticleManager>
{
    private const string PARTICLE_CSV_PATH = "/ParticlesCSV/Particles.csv"; // StreamingAssets 기준
    private Dictionary<ParticleSystem, Pool<ParticleSystem>> _sharedPools = new();
    private Dictionary<string, ParticleSystem> _particlePrefabs = new();
    public IReadOnlyDictionary<ParticleSystem, Pool<ParticleSystem>> ExplodeEffectPool => _sharedPools;
    [SerializeField] private List<DamageEntry> _damageEntries;

    private Dictionary<EDamageFloaterType, DamageNumber> _damagePrefabs = new();
    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await InitFromCsvAsync();
    }
    public UniTask InitFromCsvAsync(string relativeCsvPath = PARTICLE_CSV_PATH)
    {
        var token = this.GetCancellationTokenOnDestroy();
        foreach (var entry in _damageEntries)
        {
            if (entry != null && entry.DamagePrefab != null)
            {
                _damagePrefabs[entry.Key] = entry.DamagePrefab;
            }
        }
        return InitFromCsvAsync(relativeCsvPath, token);
    }
    public async UniTask InitFromCsvAsync(string relativeCsvPath, CancellationToken token)
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

            if (_particlePrefabs.ContainsKey(row.ParticleKey))
                continue;

            GameObject prefab = null;
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(row.AddressablePath);
                prefab = await handle.ToUniTask(cancellationToken: token);

                if (prefab == null)
                {
                    Debug.LogError($"[ParticleManager] Addressables 로드 실패: {row.AddressablePath}");
                    continue;
                }
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
            if (entry.Prefab.TryGetComponent(out Projectile projectile))
            {
                if(projectile.ExplodeEffect != null)
                {
                    if (_particlePrefabs.ContainsKey(projectile.ExplodeEffect.name))
                    {
                        continue;
                    }
                    _particlePrefabs[projectile.ExplodeEffect.name] = projectile.ExplodeEffect;
                    var (pool, parent) = GetOrCreateSharedPool(projectile.ExplodeEffect, transform);
                }
                
            }

        }
    }

    public ParticleSystem PlayByKeyLocalAsChild(string particleKey, Transform parent, Vector3 localPos, Quaternion localRot)
    {
        var prefab = GetParticlePrefab(particleKey);
        if (prefab == null) return null;

        if (!_sharedPools.TryGetValue(prefab, out var pool))
            _sharedPools[prefab] = pool = GetOrCreateSharedPool(prefab, transform).Item1;

        var instance = pool.Get();
        instance.transform.SetParent(parent, false);
        instance.transform.localPosition = localPos;
        instance.transform.localRotation = localRot;

        var autoReturn = instance.GetComponent<ParticleAutoReturn>();
        autoReturn.Init(pool);

        instance.Play();
        return instance;
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
    private ParticleSystem GetParticlePrefab(string particleKey)
    {
        if (string.IsNullOrEmpty(particleKey))
        {
            Debug.LogWarning("Particle key is null or empty. Cannot get particle prefab.");
            return null;
        }
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
    public void PlayByKey(string particleKey, Vector3 position, Quaternion rotation, bool networked)
    {
        // 네트워크 가능하면 Proxy로 RPC, 아니면 로컬 재생
        if (networked && ParticleNetworkProxy.Instance != null && ParticleNetworkProxy.Instance.Object != null && ParticleNetworkProxy.Instance.Object.IsValid)
        {
            // 클라/호스트 상관없이 요청 → 서버가 Rpc_PlayParticle 브로드캐스트
            ParticleNetworkProxy.Instance.RPC_RequestPlayParticle(particleKey, position, rotation);
        }
        else
        {
            PlayByKeyLocal(particleKey, position, rotation);
        }
    }

    public void PlayByKeyLocal(string particleKey, Vector3 position, Quaternion rotation)
    {
        var prefab = GetParticlePrefab(particleKey);
        if (prefab != null)
        {
            PlayParticle(prefab, position, rotation);
        }
        else
        {
            Debug.LogWarning($"[ParticleManager] Particle key not found: {particleKey}");
        }
    }

    public void DamageSpawn(float number, Vector3 pos, EDamageFloaterType type, bool networked)
    {
        if (networked && ParticleNetworkProxy.Instance != null && ParticleNetworkProxy.Instance.Object != null && ParticleNetworkProxy.Instance.Object.IsValid)
        {
            // 클라/호스트 상관없이 요청 → 서버가 Rpc_PlayParticle 브로드캐스트
            ParticleNetworkProxy.Instance.RPC_RequestSpawnDamage(number, pos, type);
        }
        else
        {
            SpawnDamageOnLocal(number, pos, type);
        }
    }

    private void SpawnDamageOnLocal(float number, Vector3 pos, EDamageFloaterType type)
    {
        if (_damagePrefabs.TryGetValue(type, out var prefab))
        {
            prefab.Spawn(pos, number);
        }
        else
        {
            Debug.LogWarning($"[Particle] Damage prefab not found for type: {type}");
        }
    }

}