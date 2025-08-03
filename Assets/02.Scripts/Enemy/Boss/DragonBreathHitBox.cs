using System;
using System.Collections.Generic;
using UnityEngine;

public class DragonBreathHitBox : MonoBehaviour
{
    public float ExpandSpeed = 30f;
    public float MaxLength = 20f;
    public LayerMask HitLayer;

    private BoxCollider _collider;
    private float _currentLength = 0f;
    private float _timer = 0f;
    private float _despawnTime = 3f;

    private HashSet<Collider> _hitTargets = new();
    
    private Action _onEndCallback;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    public void Init(float particleDuration, Action onDespawnCallback = null)
    {
        _collider.size = new Vector3(1f, 1f, 0f);
        _collider.center = new Vector3(0f, 0f, 0f); // center도 초기화 필요
        _timer = 0f;
        _currentLength = 0f;
        _despawnTime = particleDuration + 1f;
        _hitTargets.Clear();

        _onEndCallback = onDespawnCallback;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_currentLength < MaxLength)
        {
            _currentLength += ExpandSpeed * Time.deltaTime;
            _currentLength = Mathf.Min(_currentLength, MaxLength);

            _collider.size = new Vector3(1f, 1f, _currentLength);
            _collider.center = new Vector3(0f, 0f, _currentLength / 2f);
        }

        if (_timer >= _despawnTime)
        {
            _onEndCallback?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & HitLayer) == 0) return;
        if (_hitTargets.Contains(other)) return;

        _hitTargets.Add(other);

        // var hit = other.GetComponent<IDamageable>();
        // hit?.TakeDamage(10f, null); // Object.InputAuthority 제거됨
    }
}