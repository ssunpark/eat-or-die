using DG.Tweening;
using RaycastPro.Detectors;
using UnityEngine;

public class DragonCombat
{
    private readonly DragonController _controller;

    public DragonCombat(DragonController controller)
    {
        _controller = controller;
    }

    public void OnSpawned()
    {
        SetFightMode(_controller.IsFightMode);
    }

    public void SetFightMode(bool active)
    {
        int index = _controller.Animator.GetLayerIndex("Fight Layer");
        float weight = active ? 1f : 0f;
        _controller.Animator.SetLayerWeight(index, weight);

        if (_controller.HasStateAuthority)
        {
            _controller.RPC_SetFightLayerWeight(weight);
        }
    }

    #region Melee

    public void SetDetector(float detectRadius, float angle)
    {
        _controller.AttackDetector.Radius = detectRadius;
        _controller.AttackDetector.angleX = angle;
    }

    public void Attack()
    {
        foreach (var collider in _controller.AttackDetector.DetectedColliders)
        {
            if (collider.CompareTag("Player") && collider.TryGetComponent(out IAttackable attackable))
            {
                var attackinfo = new AttackInfo
                {
                    MeleeDamage = 10f,
                    TotalDamageMultiplier = 1f
                };
                attackable.OnHitLocal(attackinfo);
            }
        }
    }

    #endregion

    #region SpecialMelee

    private const string DarkProjectileKey = "Dragon_BlackProjectile_Prefab";
    private const string WindStormKey = "Dragon_WindStorm_Prefab";

    public void DarkProjectileEffect()
    {
        var spawnPoint = _controller.LeftPoint.position;

        var projectile = _controller.Pool.GetDirectionalPoolObject(DarkProjectileKey);

        projectile.transform.position = spawnPoint;
        var param = _controller.ParamLoader.LeftScratch_Special;
        projectile.Fire(_controller.transform.forward, param.Speed, param.LifeTime,
            () => _controller.Pool.TakeDirectionalPool(DarkProjectileKey, projectile));
    }

    public void WindStormEffect()
    {
        var spawnPoint = _controller.RightPoint.position;
        spawnPoint.y = _controller.transform.position.y;

        // Y값 제외하고 방향 계산
        var direction = spawnPoint - _controller.transform.position;
        direction.Normalize();

        var projectile = _controller.Pool.GetDirectionalPoolObject(WindStormKey);

        projectile.transform.position = spawnPoint;
        var param = _controller.ParamLoader.RightScratch_Special;
        projectile.Fire(direction, param.Speed, param.LifeTime, () => _controller.Pool.TakeDirectionalPool(WindStormKey, projectile));
    }

    #endregion

    #region Magic

    // 브레스
    public void PlayBreath(float duration)
    {
        var vfx = _controller.Pool.BreathParticlePool.Get();
        vfx.transform.position = _controller.BreathPoint.position;
        vfx.transform.rotation = Quaternion.LookRotation(_controller.transform.forward);
        vfx.Init(duration, () => _controller.Pool.BreathParticlePool.Take(vfx));
    }

    // Lava
    public void FireLava(Vector3 forward, float angle, float distance, LavaProjectileData data)
    {
        var spawnPoint = _controller.BreathPoint.position;

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 dir = rot * forward;
        Vector3 targetPos = spawnPoint + dir * distance;

        // 지면 위치 보정
        Debug.DrawRay(targetPos + Vector3.up * 5f, Vector3.down * 10f, Color.cyan, 10f);
        if (Physics.Raycast(targetPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 100f,
                LayerMask.GetMask("Floor")))
        {
            targetPos = hit.point;
        }

        var lava = _controller.Pool.LavaProjectilePool.Get();
        lava.transform.position = spawnPoint;

        lava.Fire(
            new LavaProjectileData(
                targetPos,
                data.Speed,
                data.Duration,
                data.Height),
            () => _controller.Pool.LavaProjectilePool.Take(lava),
            _controller.Pool.LavaFloorPool
        );
    }

    // Roar
    public void PerformRoarAttack(float radius, int count, float duration)
    {
        float interval = duration / count;

        // 공격 실행
        _controller.RoarExplosion.Reset(radius, count, interval);

        // 이펙트 시퀀스
        var effect = _controller.RoarEffect;
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => effect.SetActive(true))
            .Append(effect.transform.DOScale(Vector3.zero, 0f))
            .Append(effect.transform.DOScale(Vector3.one * 0.8f, duration / 4f))
            .AppendInterval(duration / 2f)
            .Append(effect.transform.DOScale(Vector3.zero, duration / 4f))
            .AppendCallback(() => effect.SetActive(false));
    }

    #endregion
}