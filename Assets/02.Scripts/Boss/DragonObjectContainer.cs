using Redcode.Pools;
using UnityEngine;
using System.Collections.Generic;

public class DragonObjectContainer : BehaviourSingleton<DragonObjectContainer>
{
    [SerializeField]
    private DragonBreathEffect _dragonBreathEffectPrefab;

    [SerializeField]
    private LavaProjectile _lavaProjectilePrefab;
    public LavaProjectile LavaProjectile => _lavaProjectilePrefab;

    [SerializeField]
    private LavaFloor _lavaFloorPrefab;
    public LavaFloor LavaFloorPrefab => _lavaFloorPrefab;

    [SerializeField]
    private BloodExplosion _bloodExplosionPrefab;
    public BloodExplosion BloodExplosionPrefab => _bloodExplosionPrefab;

    [SerializeField]
    private List<DirectionalProjectile> _directionalProjectiles;
    
    public Pool<DragonBreathEffect> BreathParticlePool { get; private set; }

    private readonly Dictionary<string, Pool<DirectionalProjectile>> _dirPools = new();

    private void Awake()
    {
        BreathParticlePool = Pool.Create(_dragonBreathEffectPrefab, 1, transform).NonLazy();

        foreach (var proj in _directionalProjectiles)
        {
            GameObject pool = new(proj.name);
            _dirPools.TryAdd(proj.name, Pool.Create(proj, 0, transform));
        }
    }

    public Pool<DirectionalProjectile> GetDirectionalPool(string key)
        => _dirPools.GetValueOrDefault(key);

    public DirectionalProjectile GetDirectionalPoolObject(string name)
        => _dirPools.TryGetValue(name, out var pool) ? pool.Get() : null;

    public void TakeDirectionalPool(string name, DirectionalProjectile projectile)
    {
        if (_dirPools.TryGetValue(name, out var pool))
        {
            pool.Take(projectile);
        }
    }
}