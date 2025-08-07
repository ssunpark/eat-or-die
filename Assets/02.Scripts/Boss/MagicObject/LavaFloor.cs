using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    private const float REDUCE_FACTOR = 5f;
    private const float UP_FACTOR = 2f;
    
    private float _timer;
    private float _duration;
    private bool _isActive;
    
    private EffectVisual _effect;
    public EffectVisual Effect => _effect;

    private void Awake()
    {
        _effect = GetComponentInChildren<EffectVisual>();
    }

    public void Init(float duration)
    {
        _duration = duration;
        _timer = 0f;
        _isActive = true;
        _effect.Reset(duration, REDUCE_FACTOR, UP_FACTOR);
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