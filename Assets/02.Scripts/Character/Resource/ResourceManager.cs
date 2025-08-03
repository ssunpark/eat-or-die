using System;
using UnityEngine;

// Todo: DDD로 변경
public class ResourceManager
{
    private readonly StatManager _stat;

    public float CurrentHunger { get; private set; }
    public float MaxHunger { get; private set; }
    public float CurrentMana { get; private set; }

    public float MaxMana { get; private set; }

    public event Action<float, float> OnHungerChanged;
    public event Action<float, float> OnManaChanged;

    public ResourceManager(StatManager stat)
    {
        _stat = stat;
        MaxHunger = CurrentHunger = _stat.GetStat(EStatType.MaxHunger);
        MaxMana = CurrentMana = _stat.GetStat(EStatType.MaxMana);
    }
    public void ConsumeHunger(float amount)
    {
        CurrentHunger = Mathf.Max(CurrentHunger - amount, 0f);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }

    public void RestoreHunger(float amount)
    {
        MaxHunger = _stat.GetStat(EStatType.MaxHunger);
        CurrentHunger = Mathf.Min(CurrentHunger + amount, MaxHunger);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }


    public void RestoreMana(float amount)
    {
        MaxMana = _stat.GetStat(EStatType.MaxMana);
        CurrentMana = Mathf.Min(CurrentMana + amount, MaxMana);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }
    public void ConsumeMana(float amount)
    {
        CurrentMana = Mathf.Max(CurrentMana - amount, 0f);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public bool HasEnoughMana(float amount)
    {
        return CurrentMana >= amount;
    }

    public void ResetAll()
    {
        SetHunger(_stat.GetStat(EStatType.MaxHunger));
        SetMana(_stat.GetStat(EStatType.MaxMana));
    }

    public void SetMana(float value)
    {
        MaxMana = _stat.GetStat(EStatType.MaxMana);
        CurrentMana = Mathf.Clamp(value, 0f, MaxMana);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }
    public void SetHunger(float value)
    {
        MaxHunger = _stat.GetStat(EStatType.MaxHunger);
        CurrentHunger = Mathf.Clamp(value, 0f, MaxHunger);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }

    internal float GetHungerPercent()
    {
        return (CurrentHunger / MaxHunger);
    }
}