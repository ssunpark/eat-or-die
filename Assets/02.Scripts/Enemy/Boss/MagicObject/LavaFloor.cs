using System;
using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    private float _timer;
    private float _duration;
    private Action _onDespawnCallback;
    private bool _isActive;

    public void Init(float duration, Action onDespawnCallback)
    {
        _duration = duration;
        _onDespawnCallback = onDespawnCallback;
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
            _onDespawnCallback?.Invoke();
        }
    }
}