using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Detectors;
using Redcode.Pools;
using UnityEngine;

public class BloodExplosion : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> _mainParticleList = new List<ParticleSystem>();
    [SerializeField]
    private EffectVisualController _effectController;
    [SerializeField]
    private GameObject _explosion;

    private RangeDetector _rangeDetector;
    
    private Coroutine _explosionCoroutine;

    private void Awake()
    {
        _rangeDetector = GetComponent<RangeDetector>();
    }

    // 폭발 준비
    public void StartExplosion(float delay, float targetSize, Pool<BloodExplosion> pool)
    {
        transform.localScale = Vector3.one * 0.5f;
        
        foreach (var particle in _mainParticleList)
        {
            var main = particle.main;
            main.duration = delay;
        }

        foreach (var particle in _mainParticleList)
        {
            particle.Play();
        }
        
        gameObject.SetActive(true);

        if (_explosionCoroutine != null)
        {
            StopCoroutine(_explosionCoroutine);
        }
        _explosionCoroutine = StartCoroutine(Explode(delay));

        transform.DOScale(targetSize, delay);
        
        _effectController.SetEndCallBack(() => pool.Take(this));
    }

    private IEnumerator Explode(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _explosion.SetActive(true);
        Debug.Log(_rangeDetector.DetectedColliders.Count);
        foreach (var collider in _rangeDetector.DetectedColliders)
        {
            if (collider.TryGetComponent(out IAttackable attackable))
            {
                var attackinfo = new AttackInfo
                {
                    MeleeDamage = 10f,
                    TotalDamageMultiplier = 1f
                };
                attackable.OnHitLocal(attackinfo);
            }
        }
    }
}
