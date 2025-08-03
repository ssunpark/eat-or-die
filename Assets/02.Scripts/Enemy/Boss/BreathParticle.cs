using System;
using UnityEngine;

public class BreathParticle : MonoBehaviour
{
    private Action _onEndCallback;
    
    public void Init(Action onDespawnCallback = null)
    {
        _onEndCallback = onDespawnCallback;
    }
    
    private void OnParticleSystemStopped()
    {
        _onEndCallback?.Invoke();
    }
}