using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class LavaFloor : NetworkBehaviour
{
    [Networked]
    public Vector3 StartPosition { get; set; }
    [Networked]
    public int StartTick { get; set; }
    [Networked]
    public float Duration { get; set; }

    [SerializeField]
    private float _appearSeconds = 1f;
    [SerializeField]
    private float _fadeSeconds = 1f;

    private EffectVisualController _effect;
    private Collider _col;
    private bool _vfxStarted;
    
    public float _damage;
    public void SetDamage(float damage) => _damage = damage;

    public override void Spawned()
    {
        transform.position = StartPosition;
        
        _col = GetComponent<Collider>();
        _effect = GetComponentInChildren<EffectVisualController>();
        if (_col)
            _col.enabled = true;
        _vfxStarted = false; // 여기서는 아직 시작하지 않음
    }

    public override void Render()
    {
        float elapsed = (Runner.Tick - StartTick) * Runner.DeltaTime;
        if (elapsed < 0f)
            return;

        if (!_vfxStarted && _effect != null)
        {
            float hold = Mathf.Max(0f, Duration - _appearSeconds - _fadeSeconds);
            _effect.Appear(hold, _appearSeconds, _fadeSeconds);
            MasterAudio.PlaySound3DAtTransform("LavaExplosion", transform);
            _vfxStarted = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        float elapsed = (Runner.Tick - StartTick) * Runner.DeltaTime;
        if (Object.HasStateAuthority && elapsed >= Duration)
        {
            Runner.Despawn(Object); // 페이드가 딱 종료되는 시점
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 데미지 판정은 권위만
        if (!Object || !Object.HasStateAuthority)
            return;
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent(out IAttackable hit))
        {
            hit.OnHitLocal(new AttackInfo
            {
                MeleeDamage = _damage,
                TotalDamageMultiplier = 1f
            });
        }
    }
}