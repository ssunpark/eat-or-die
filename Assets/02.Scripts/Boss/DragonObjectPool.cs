using Redcode.Pools;
using UnityEngine;
using System.Collections.Generic;

public class DragonObjectPool
{
    public Pool<DragonBreathEffect> BreathParticlePool { get; private set; }
    public Pool<BloodExplosion> BloodExplosionPool { get; private set; }

    private readonly Dictionary<string, Pool<DirectionalProjectile>> _dirPools = new();

    public DragonObjectPool(DragonController controller)
    {
        Transform root = controller.transform;
        GameObject DragonEffectPool = new("DragonEffectPool");

        BreathParticlePool = Pool.Create(controller.DragonBreathEffectPrefab, 1, root).NonLazy();
        BloodExplosionPool = Pool.Create(controller.BloodExplosionPrefab, 0, DragonEffectPool.transform);

        foreach (var proj in controller.DirectionalProjectiles)
        {
            GameObject pool = new(proj.name);
            _dirPools.TryAdd(proj.name, Pool.Create(proj, 0, pool.transform));
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