using Redcode.Pools;
using UnityEngine;
using System.Collections.Generic;

public class DragonObjectPool
{
    public Pool<DragonBreathEffect> BreathParticlePool { get; private set; }
    public Pool<LavaProjectile> LavaProjectilePool { get; private set; }
    public Pool<LavaFloor> LavaFloorPool { get; private set; }

    private readonly Dictionary<string, Pool<DirectionalProjectile>> _dirPools = new();

    public DragonObjectPool(DragonController controller)
    {
        Transform root = controller.transform;
        GameObject lavaPool = new("LavaPool");
        
        BreathParticlePool = Pool.Create(controller.DragonBreathEffectPrefab, 0, root);
        LavaProjectilePool = Pool.Create(controller.LavaProjectile, 0, lavaPool.transform);
        LavaFloorPool = Pool.Create(controller.LavaFloorPrefab, 0, lavaPool.transform);

        foreach (var proj in controller.GetComponentsInChildren<DirectionalProjectile>())
        {
            GameObject pool = new(proj.name);
            _dirPools.TryAdd(proj.name, Pool.Create(proj, 0, pool.transform));
        }
    }

    public Pool<DirectionalProjectile> GetDirectionalPool(string name)
        => _dirPools.TryGetValue(name, out var pool) ? pool : null;
}