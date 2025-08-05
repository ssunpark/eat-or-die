using System;
using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    private const float REDUCE_FACTOR = 5f;
    private const float UP_FACTOR = 2f;
    
    private float _timer;
    private float _duration;
    private bool _isActive;
    
    private LavaVisual _lava;
    public LavaVisual Lava => _lava;

    private void Awake()
    {
        _lava = GetComponentInChildren<LavaVisual>();
    }

    public void Init(float duration)
    {
        _duration = duration;
        _timer = 0f;
        _isActive = true;
        _lava.Reset(duration, REDUCE_FACTOR, UP_FACTOR);
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
}