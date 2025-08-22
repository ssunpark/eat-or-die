using Fusion;
using UnityEngine;
using DamageNumbersPro;
using System.Collections.Generic;

public enum EDamageFloaterType : byte
{
    Damage = 0,
    Heal = 1,
    Experience = 2,
    Critical = 3,
    Gold = 4
}
public class ParticleNetworkProxy : NetworkBehaviour
{
    public static ParticleNetworkProxy Instance { get; private set; }

    [System.Serializable]
    public class DamageEntry
    {
        public EDamageFloaterType Key;
        public DamageNumber DamagePrefab;
    }

    [SerializeField] private List<DamageEntry> _damageEntries;

    private Dictionary<EDamageFloaterType, DamageNumber> _damagePrefabs = new();

    public override void Spawned()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in _damageEntries)
        {
            if (entry != null  && entry.DamagePrefab != null)
            {
                _damagePrefabs[entry.Key] = entry.DamagePrefab;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayParticle(string key, Vector3 position, Quaternion rotation)
    {
        ParticleManager.Instance?.PlayByKeyLocal(key, position, rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestPlayParticle(string key, Vector3 position, Quaternion rotation)
    {
        Rpc_PlayParticle(key, position, rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestSpawnDamage(float damage, Vector3 position, EDamageFloaterType floaterType)
    {
        RPC_SpawnDamage(damage, position, floaterType);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnDamage(float damage, Vector3 position, EDamageFloaterType floaterType)
    {
        if (_damagePrefabs.TryGetValue(floaterType, out var prefab))
        {
            prefab.Spawn(position, damage);
        }
        else
        {
            Debug.LogWarning($"[ParticleNetworkProxy] Damage prefab not found for type: {floaterType}");
        }
    }
}