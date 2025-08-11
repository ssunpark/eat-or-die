using System;
using UnityEngine;

public class DragonStats
{
    private readonly DragonController _controller;
    public float MaxHP { get; private set; }
    public float CurrentHP => _controller.CurrentHeath;

    public DragonStats(DragonController controller)
    {
        _controller = controller;
        MaxHP = _controller.ParamLoader.Base.HP;
    }

    public void OnSpawned()
    {
        _controller.CurrentHeath = MaxHP;
    }

    public void TakeDamage(float amount)
    {
        _controller.CurrentHeath = Mathf.Max(CurrentHP - amount, 0f);
    }
    
    public bool IsDead => CurrentHP <= 0f;
}