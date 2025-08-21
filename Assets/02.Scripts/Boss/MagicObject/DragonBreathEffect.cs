using System;
using System.Linq;
using DarkTonic.MasterAudio;
using RaycastPro.Detectors;
using UnityEngine;

public class DragonBreathEffect : MonoBehaviour
{
    [Header("공격 설정")]
    public float ExpandSpeed = 30f;
    public float MaxLength = 20f;

    [Header("파티클 설정")]
    [SerializeField]
    private ParticleSystem _mainParticle;
    [SerializeField]
    private ParticleSystem _subParticle;

    [SerializeField, Header("플레이어 감지")]
    private SightDetector _detector;
    
    private float _currentRadius = 0f;
    private float _timer = 0f;
    private float _despawnTime = 0f;
    private float _damage;
    private Action _onEndCallback;

    private Transform _dragon;

    private bool _isState;

    public void Init(float particleDuration, bool isState, float damage, Transform dragon, Action onDespawnCallback = null)
    {
        _isState = isState;
        _onEndCallback = onDespawnCallback;
        _timer = 0f;
        _currentRadius = 0f;
        _despawnTime = particleDuration;
        _damage = damage;
        _dragon = dragon;
        
        // 파티클 재생
        var main1 = _mainParticle.main;
        main1.duration = particleDuration;

        if (_subParticle != null)
        {
            var main2 = _subParticle.main;
            main2.duration = particleDuration;
            _subParticle.Play();
        }
        
        _mainParticle.Play();
        
        // 콜라이더 초기화
        if (!_isState)
        {
            _detector.enabled = false;
            return;
        }
        _detector.enabled = true;
        _detector.radius = 0f;
        _detector.collectLOS = true;
    }

    private void Update()
    {
        if (!_isState)
        {
            return;
        }
        _timer += Time.deltaTime;

        // 히트박스 점점 커지게
        if (_currentRadius < MaxLength)
        {
            _currentRadius += ExpandSpeed * Time.deltaTime;
            _currentRadius = Mathf.Min(_currentRadius, MaxLength);

            _detector.radius = _currentRadius;
        }

        if (_timer > _despawnTime)
        {
            _detector.enabled = false;
            MasterAudio.StopSoundGroupOfTransform(_dragon, "Breath");
        }

        if (!_detector.enabled)
        {
            return;
        }
        
        foreach (var collider in _detector.DetectedColliders)
        {
            Attack(collider.gameObject);
        }
    }

    private void Attack(GameObject player)
    {
        if (!_isState)
        {
            return;
        }

        if (player.TryGetComponent(out IAttackable hit))
        {
            var attackinfo = new AttackInfo
            {
                MeleeDamage = _damage,
                TotalDamageMultiplier = 1f
            };
            hit.OnHitLocal(attackinfo);
        }
    }

    // 파티클이 중간에 멈춘 경우에도 콜백 보장
    private void OnParticleSystemStopped()
    {
        _onEndCallback?.Invoke();
    }
}