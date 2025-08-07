using System;
using UnityEngine;

public class DragonBreathEffect : MonoBehaviour
{
    [Header("히트박스 설정")]
    public float ExpandSpeed = 30f;
    public float MaxLength = 20f;

    [Header("파티클 설정")]
    [SerializeField]
    private ParticleSystem _mainParticle;
    [SerializeField]
    private ParticleSystem _subParticle;

    private BoxCollider _collider;
    private float _currentLength = 0f;
    private float _timer = 0f;
    private float _despawnTime = 0f;
    private Action _onEndCallback;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    public void Init(float particleDuration, Action onDespawnCallback = null)
    {
        _onEndCallback = onDespawnCallback;
        _timer = 0f;
        _currentLength = 0f;
        _despawnTime = particleDuration;
        
        // 콜라이더 초기화
        _collider.enabled = true;
        var size = _collider.size;
        size.z = 0f;
        _collider.size = size;
        _collider.center = new Vector3(0f, 0f, 0f);

        // 파티클 재생
        var main1 = _mainParticle.main;
        main1.duration = particleDuration;
        _mainParticle.Play();

        if (_subParticle != null)
        {
            var main2 = _subParticle.main;
            main2.duration = particleDuration;
            _subParticle.Play();
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // 히트박스 점점 커지게
        if (_currentLength < MaxLength)
        {
            _currentLength += ExpandSpeed * Time.deltaTime;
            _currentLength = Mathf.Min(_currentLength, MaxLength);

            _collider.size = new Vector3(_collider.size.x, _collider.size.y, _currentLength);
            _collider.center = new Vector3(0f, 0f, _currentLength / 2f);
        }

        if (_timer > _despawnTime)
        {
            _collider.enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var hit = other.GetComponent<IAttackable>();
        if (hit != null)
        {
            var attackinfo = new AttackInfo
            {
                MeleeDamage = 10f,
                TotalDamageMultiplier = 1f
            };
            hit.OnHitLocal(attackinfo, null);
        }
    }

    // 파티클이 중간에 멈춘 경우에도 콜백 보장
    private void OnParticleSystemStopped()
    {
        _onEndCallback?.Invoke();
    }
}