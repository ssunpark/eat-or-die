using System;
using System.Collections.Generic;
using UnityEngine;

public class DragonBreathHitBox : MonoBehaviour
{
    public float ExpandSpeed = 30f;
    public float MaxLength = 20f;

    private BoxCollider _collider;
    private float _currentLength = 0f;
    private float _timer = 0f;
    private float _despawnTime = 3f;
    
    private Action _onEndCallback;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    public void Init(float particleDuration, Action onDespawnCallback = null)
    {
        var newSize = _collider.size;
        newSize.z = 0f;
        _collider.size = newSize;
        _collider.center = new Vector3(0f, 0f, 0f); // center도 초기화 필요
        _timer = 0f;
        _currentLength = 0f;
        _despawnTime = particleDuration + 1f;

        _onEndCallback = onDespawnCallback;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_currentLength < MaxLength)
        {
            _currentLength += ExpandSpeed * Time.deltaTime;
            _currentLength = Mathf.Min(_currentLength, MaxLength);

            _collider.size = new Vector3(_collider.size.x, _collider.size.y, _currentLength);
            _collider.center = new Vector3(0f, 0f, _currentLength / 2f);
        }

        if (_timer >= _despawnTime)
        {
            _onEndCallback?.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        var hit = other.GetComponent<IAttackable>();
        var attackinfo = new AttackInfo();
        attackinfo.MeleeDamage = 10f;
        attackinfo.TotalDamageMultiplier = 1f;
        hit.OnHitLocal(attackinfo, null);
    }
}