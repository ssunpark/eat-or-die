using Fusion;
using Redcode.Pools;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    public LayerMask HitMask;
    public float Speed = 10f;
    public float Lifetime = 5f;
    private AttackInfo _attackInfo;
    private Collider _collider;
    [Networked] private float _magicDamage { get; set; }
    [Networked] private float _meleeDamage { get; set; }
    [Networked] private Vector3 _knockbackVector { get; set; }
    [Networked] private float _hitRecoveryTime { get; set; }
    [Networked] private float _bossDamageMultiplier { get; set; }
    [Networked] private float _totalDamageMultiplier { get; set; }
    [Networked] private NetworkObject Attacker { get; set; }
    public ParticleSystem ExplodeEffect;
    [Networked] private TickTimer LifeTimer { get; set; }
    [Networked]
    private bool _isHit_networked { get; set; }

    bool _isHit;
    IAttackable _hitObject;
    [Networked, OnChangedRender(nameof(TargetSet))] private NetworkObject _target { get; set; }

    public void TargetSet()
    {
        if (_target == null)
        {
            Debug.LogWarning("Target is null, cannot set hit object.");
            _hitObject = null;
            return;
        }
        _hitObject = _target.GetComponent<IAttackable>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, Lifetime);
            _target = null;
        }
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _isHit = false;
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

        if (_isHit_networked)
        {

            if (_target != null)
            {
                _hitObject = _target.GetComponent<IAttackable>();
            }

            if (_hitObject != null)
            {
                _attackInfo.MagicDamage = _magicDamage;
                _attackInfo.MeleeDamage = _meleeDamage;
                _attackInfo.KnockbackVector = _knockbackVector;
                _attackInfo.HitRecoveryTime = _hitRecoveryTime;
                _attackInfo.BossDamageMultiplier = _bossDamageMultiplier;
                _attackInfo.TotalDamageMultiplier = _totalDamageMultiplier;
                _attackInfo.Attacker = Attacker;
                _hitObject.OnHitLocal(_attackInfo);
            }
            else
            {
                Debug.LogWarning("HitObject is null when OnHitChanged is called.");
            }
            if (Runner.IsServer)
            {
                DestroySelf();
            }
        }
    }
    public void Initialize(AttackInfo attackInfo, LayerMask layerMask)
    {
        if (Runner.IsServer == false)
            return;
        _hitObject = null;
        _target = null;
        _attackInfo = attackInfo;
        _magicDamage = attackInfo.MagicDamage;
        _meleeDamage = attackInfo.MeleeDamage;
        _knockbackVector = attackInfo.KnockbackVector;
        _hitRecoveryTime = attackInfo.HitRecoveryTime;
        _bossDamageMultiplier = attackInfo.BossDamageMultiplier;
        _totalDamageMultiplier = attackInfo.TotalDamageMultiplier;
        Attacker = attackInfo.Attacker;
        _collider.enabled = true;
        _isHit_networked = false;
        HitMask = layerMask;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isHit_networked)
        {
            Debug.LogWarning("Projectile has already hit something, ignoring further collisions.");
            return;
        }
        if ((HitMask.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log($"Projectile {gameObject.name} hit an object not in HitMask: {other.gameObject.name}");
            return;
        }
        if (other.TryGetComponent<IAttackable>(out var target))
        {
            if (target.NetworkObject == Attacker)
            {
                Debug.LogWarning($"Projectile {gameObject.name} hit its own attacker: {target.NetworkObject.name}");
                return;
            }
            if (Runner.IsServer)
            {
                _target = target.NetworkObject;
                _collider.enabled = false;
                _isHit_networked = true;
                ParticleManager.Instance.RpcPlayParticle(ExplodeEffect.name, transform.position, Quaternion.identity);
            }
        }
    }

    private void DestroySelf()
    {
        Runner.Despawn(Object);
    }
}
