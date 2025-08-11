using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class BloodExplosion : NetworkBehaviour
{
    [Header("VFX")]
    [SerializeField]
    private List<ParticleSystem> _mainParticleList = new();
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private float _startScale = 0.5f;

    [Header("Gameplay")]
    [SerializeField]
    private float _radius = 3f;
    [SerializeField]
    private LayerMask _attackMask;

    // 네트워크 동기화되는 파라미터
    [Networked]
    public Vector3 StartPosition { get; set; }
    [Networked]
    private TickTimer ExplosionTimer { get; set; } // 서버가 정한 "데미지 시점"
    [Networked]
    public float Duration { get; set; }
    [Networked]
    public float RemainDuration { get; set; } // VFX 전체 지연(초)
    [Networked]
    public float TargetScale { get; set; } // 최종 스케일

    private bool _damaged;
    private Tween _scaleTween;

    private float _timer;

    public override void Spawned()
    {
        transform.position = StartPosition;
        ExplosionTimer = TickTimer.CreateFromSeconds(Runner, Duration);
        // VFX 초기화 (모든 클라)
        transform.localScale = Vector3.one * _startScale;
        _explosion.SetActive(false);
        _damaged = false;

        foreach (var ps in _mainParticleList)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.duration = Duration;
            ps.Play();
        }

        // DOTween은 "진행도 시킹"으로 제어할 예정이므로 일단 멈춘 트윈 생성
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(TargetScale, Duration)
            .SetEase(Ease.Linear);
    }

    public override void FixedUpdateNetwork()
    {
        if (!_damaged && ExplosionTimer.Expired(Runner))
        {
            TriggerExplosion();
        }

        // 흔적은 간단하게 로컬 타임으로
        _timer += Time.deltaTime;
        if (_timer > RemainDuration)
        {
            Runner.Despawn(Object);
        }
    }

    private void TriggerExplosion()
    {
        _damaged = true;
        _explosion.SetActive(true);

        if (HasStateAuthority)
            DoDamageServer();
    }

    private void DoDamageServer()
    {
        Collider[] hits = new Collider[8];
        var hitCount = Physics.OverlapSphereNonAlloc(transform.position, _radius, hits, _attackMask);

        if (hitCount <= 0)
        {
            return;
        }

        foreach (var h in hits)
        {
            if (h == null)
            {
                break;
            }

            if (h.TryGetComponent(out IAttackable atk))
            {
                atk.OnHitLocal(new AttackInfo { MeleeDamage = 10f, TotalDamageMultiplier = 1f });
            }
        }
    }

    private void OnDisable()
    {
        _scaleTween?.Kill();
        _damaged = false;
    }
}