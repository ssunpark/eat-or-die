using System;
using UnityEngine;

public class DragonStats
{
    private readonly DragonController _controller;
    public float MaxHP { get; private set; }
    public float CurrentHP => _controller.CurrentHealth;

    public DragonStats(DragonController controller)
    {
        _controller = controller;
        MaxHP = _controller.ParamLoader.Base.HP;
    }

    public void OnSpawned()
    {
        _controller.CurrentHealth = MaxHP;
    }

    public void TakeDamage(float amount)
    {
        _controller.CurrentHealth = Mathf.Max(CurrentHP - amount, 0f);
    }
    
    public bool IsDead => CurrentHP <= 0f;
}