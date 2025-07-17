using System;
using UnityEngine;
public class ResourceManager
{
    private readonly StatManager _stat;

    public float CurrentHealth { get; private set; }
    public float CurrentSatiety { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnSatietyChanged;

    public ResourceManager(StatManager stat)
    {
        _stat = stat;
        CurrentHealth = _stat.GetStat(EStatType.MaxHealth);
        CurrentSatiety = _stat.GetStat(EStatType.MaxSatiety);
    }

    public void ConsumeHealth(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxHealth);
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnHealthChanged?.Invoke(CurrentHealth, max);
    }

    public void RestoreHealth(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxHealth);
        CurrentHealth = Mathf.Min(CurrentHealth + amount, max);
        OnHealthChanged?.Invoke(CurrentHealth, max);
    }

    public void ConsumeSatiety(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxSatiety);
        CurrentSatiety = Mathf.Max(CurrentSatiety - amount, 0f);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }

    public void RestoreSatiety(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxSatiety);
        CurrentSatiety = Mathf.Min(CurrentSatiety + amount, max);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }
}