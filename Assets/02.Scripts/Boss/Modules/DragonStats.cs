using System;
using UnityEngine;

public class DragonStats
{
    public float MaxHP { get; private set; }
    public float CurrentHP { get; private set; }

    public DragonStats(DragonStateParameterSet.BaseParams baseParams)
    {
        MaxHP = baseParams.HP;
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