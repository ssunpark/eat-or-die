using System;
using DG.Tweening;
using Fusion;
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
        
        if (!_controller.HasStateAuthority)
        {
            _controller.AttackDetector.enabled = false;
        }
    }

    public void SetFightMode(bool active)
    {
        _controller.RPC_SetFightLayerWeight(active);
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
        projectile.Fire(direction, param.Speed, param.LifeTime,
            () => _controller.Pool.TakeDirectionalPool(WindStormKey, projectile));
    }

    #endregion

    #region Magic

    public TickTimer BreathTimer { get => _controller.BreathTimer; set => _controller.BreathTimer = value; }
    // 브레스
    public void PlayBreath(float duration)
    {
        var vfx = _controller.Pool.BreathParticlePool.Get();
        vfx.transform.position = _controller.BreathPoint.position;
        vfx.transform.rotation = Quaternion.LookRotation(_controller.transform.forward);
        vfx.Init(duration, _controller.HasStateAuthority, () => _controller.Pool.BreathParticlePool.Take(vfx));
    }

    // Lava
    public void FireLava(Vector3 forward, float angle, float distance, LavaProjectileData data)
    {
        var spawnPoint = _controller.BreathPoint.position;

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 dir = rot * forward;
        Vector3 targetPos = spawnPoint + dir * distance;

        // 지면 보정
        if (Physics.Raycast(targetPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 100f,
                LayerMask.GetMask("Floor")))
            targetPos = hit.point;

        // 권위에서만 네트워크 스폰
        if (_controller.HasStateAuthority)
        {
            // 1~2틱 리드로 여유 (네트 시간 보정)
            int startTick = _controller.Runner.Tick + 2;

            var proj = _controller.Runner.Spawn(
                _controller.LavaProjectile, // NetworkObject 붙은 프리팹
                spawnPoint,
                Quaternion.identity,
                onBeforeSpawned: (runner, obj) =>
                {
                    var proj = obj.GetComponent<LavaProjectile>();
                    // 네트워크 필드들을 "스폰 스냅샷"에 포함
                    proj.StartPosition = spawnPoint;
                    proj.TargetPos = targetPos;
                    proj.Speed = data.Speed;
                    proj.Height = data.Height;
                    proj.Duration = data.Duration;
                    proj.StartTick = startTick;
                });

            // 도착 시 할 일
            Action onArrived = () =>
            {
                // 도착 위치에 네트워크 LavaFloor 스폰 (권위만)
                _controller.Runner.Spawn(
                    _controller.LavaFloorPrefab, targetPos, Quaternion.identity,
                    onBeforeSpawned: (runner, obj) =>
                    {
                        var proj = obj.GetComponent<LavaFloor>();
                        proj.StartPosition = targetPos;
                        proj.StartTick = _controller.Runner.Tick;
                        proj.Duration = data.Duration;
                    });
            };

            proj.SetArrivedAction(onArrived);
        }
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

    public void PerformBloodExplode(float duration, float targetSize, float remainDuration)
    {
        var explosion = _controller.Runner.Spawn(_controller.BloodExplosionPrefab, _controller.transform.position, Quaternion.identity,
            onBeforeSpawned: (runner, obj) =>
        {
            var proj = obj.GetComponent<BloodExplosion>();
            proj.Duration = duration;
            proj.TargetScale = targetSize;
            proj.RemainDuration = remainDuration;
        });
    }

    #endregion
}