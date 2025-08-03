using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DragonBreath : NetworkBehaviour
{
    [SerializeField]
    private ParticleSystem fireParticle;
    [SerializeField]
    private ParticleSystem subParticle;

    public float ExpandSpeed = 30f;
    public float MaxLength = 20f;
    public LayerMask HitLayer;

    private BoxCollider _collider;
    private float _currentLength = 0f;
    private float _timer = 0f;
    private float _despawnTime = 3f;

    private HashSet<Collider> _hitTargets = new();

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;

        _collider.size = new Vector3(1f, 1f, 0f);
        _collider.center = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// 파티클 재생 시간 기반으로 duration 설정 및 파티클 재생
    /// </summary>
    public void Init(float particleDuration)
    {
        _timer = 0f;
        _despawnTime = particleDuration + 1f;

        if (fireParticle != null)
        {
            var main = fireParticle.main;
            main.duration = particleDuration;
            fireParticle.Play();
        }

        if (subParticle != null)
        {
            var main = subParticle.main;
            main.duration = particleDuration;
            subParticle.Play();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        _timer += Runner.DeltaTime;

        if (_currentLength < MaxLength)
        {
            _currentLength += ExpandSpeed * Runner.DeltaTime;
            _currentLength = Mathf.Min(_currentLength, MaxLength);

            _collider.size = new Vector3(1f, 1f, _currentLength);
            _collider.center = new Vector3(0f, 0f, _currentLength / 2f);
        }

        if (_timer >= _despawnTime)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & HitLayer) == 0) return;
        if (_hitTargets.Contains(other)) return;

        _hitTargets.Add(other);

        var hit = other.GetComponent<IDamageable>();
        hit?.TakeDamage(10f, Object.InputAuthority);
    }
}
