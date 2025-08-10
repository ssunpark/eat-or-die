using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    private float _timer;
    private float _duration;
    private bool _isActive;
    
    private EffectVisualController _effect;
    public EffectVisualController Effect => _effect;

    private void Awake()
    {
        _effect = GetComponentInChildren<EffectVisualController>();
    }

    public void Init(float duration)
    {
        _duration = duration;
        _timer = 0f;
        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive)
            return;

        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            _isActive = false;
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
}