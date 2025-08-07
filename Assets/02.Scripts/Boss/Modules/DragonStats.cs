using System;
using UnityEngine;

public class DragonStats
{
    private readonly DragonController _controller;
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }

    public DragonStats(DragonController controller)
    {
        _controller = controller;
        MaxHP = _controller.ParamLoader.Base.HP;
        CurrentHP = MaxHP;
    }

    public void TakeDamage(float amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, 0f);
    }

    public void Heal(float amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    public bool IsDead => CurrentHP <= 0f;
}