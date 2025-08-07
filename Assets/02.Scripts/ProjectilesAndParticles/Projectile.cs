using Fusion;
using Redcode.Pools;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private LayerMask _hitMask;
    public float Speed = 10f;
    public float Lifetime = 5f;
    private AttackInfo _attackInfo;
    private NetworkObject _attacker;
    private Collider _collider;
    [SerializeField] private ParticleSystem _explodeEffect;
    [Networked] private TickTimer LifeTimer { get; set; }
    private Pool<ParticleSystem> _pool;


    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, Lifetime);
        }
        _collider = GetComponent<Collider>();
        _pool = ProjectileManager.Instance.ExplodeEffectPool[_explodeEffect];
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            transform.position += transform.forward * Speed * Runner.DeltaTime;

            if (LifeTimer.Expired(Runner))
            {
                DestroySelf();
            }
        }

        if (_isHit)
        {
            _collider.enabled = false;
            _isHit = false;
            _hitObject?.OnHitLocal(_attackInfo);
            if (_pool == null)
            {
                Debug.LogWarning("[Projectile] _pool is null. Make sure the projectile prefab is properly initialized.");
                return;
            }
            PlayParticle();

            if (Object.HasStateAuthority)
            {
                DestroySelf();
            }
            return;
        }
    }

    private void PlayParticle()
    {
        var ps = _pool.Get();
        ps.transform.position = transform.position;
        ps.transform.rotation = transform.rotation;

        var autoReturn = ps.GetComponent<ParticleAutoReturn>();
        autoReturn.Init(_pool);

        ps.Play();
    }

    public void Initialize(AttackInfo attackInfo)
    {
        _attackInfo = attackInfo;
        _attacker = attackInfo.Attacker;
    }

    bool _isHit;
    IAttackable _hitObject;

    private void OnTriggerEnter(Collider other)
    {
        if (_isHit)
        {
            return;
        }
        if ((_hitMask.value & (1 << other.gameObject.layer)) == 0)
            return;
        if (other.TryGetComponent<IAttackable>(out var target))
        {
            if (target.NetworkObject == _attacker) return;
            _isHit = true;
            _hitObject = target;
        }
    }

    private void DestroySelf()
    {
        Runner.Despawn(Object);
    }
}
