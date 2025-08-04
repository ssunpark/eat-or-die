using System;
using UnityEngine;

public class BreathParticle : MonoBehaviour
{
    private Action _onEndCallback;

    [SerializeField]
    private ParticleSystem _particle;
    [SerializeField]
    private ParticleSystem _subParticle;

    public void Init(float duration, Action onDespawnCallback = null)
    {
        _onEndCallback = onDespawnCallback;

        // duration 설정
        var main1 = _particle.main;
        main1.duration = duration;

        if (_subParticle != null)
        {
            var main2 = _subParticle.main;
            main2.duration = duration;
        }

        // 파티클 재생 (필요 시)
        _particle?.Play();
        _subParticle?.Play();
    }

    private void OnParticleSystemStopped()
    {
        _onEndCallback?.Invoke();
    }
}