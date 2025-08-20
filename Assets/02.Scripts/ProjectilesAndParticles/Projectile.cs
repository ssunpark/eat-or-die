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
    [SerializeField] private bool _areYouPlayersProjectileAndMagicProjectile = false;
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
    private NetworkBool _isHit_networked { get; set; }



    bool _spawned;
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
            _isHit_networked = false;

        }
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _spawned = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.IsValid == false || !_spawned) return;
        if (Object.HasStateAuthority)
        {
            transform.position += transform.forward * Speed * Runner.DeltaTime;

            if (LifeTimer.Expired(Runner))
            {
                DestroySelf();
                return;
            }
        }
        if (Object.IsValid && _isHit_networked)
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
                if (_areYouPlayersProjectileAndMagicProjectile)
                {
                    RPC_GrantExpOrder(Attacker.InputAuthority, "MagicAttackHit");
                }
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
            return;
        }
        else if (other.TryGetComponent<IAttackable>(out var target))
        {
            if (target.NetworkObject == Attacker)
            {
                // 공격자 자신은 안때림
                return;
            }
            if((HitMask.value & (1 << other.gameObject.layer)) == 0)
            {
                // 마스크에 맞지 않는 레이어는 안때림
                return;
            }
            if (Runner.IsServer)
            {
                _target = target.NetworkObject;
            }
        }
        if (Runner.IsServer)
        {

            _collider.enabled = false;
            _isHit_networked = true;
            ParticleManager.Instance.PlayByKey(ExplodeEffect.name, transform.position, Quaternion.identity, true);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    protected void RPC_GrantExpOrder([RpcTarget] PlayerRef player, string actionName)
    {
        Attacker.GetComponent<Player>().ExpHandler.GrantExp(actionName);
    }
    private void DestroySelf()
    {
        _spawned = false;
        Runner.Despawn(Object);
    }
}
