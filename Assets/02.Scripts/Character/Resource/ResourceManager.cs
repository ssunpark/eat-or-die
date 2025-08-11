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

        _stat.RegisterModifierCallback(EStatType.MaxHunger, OnMaxHungerChanged, OnMaxHungerChanged);
        _stat.RegisterModifierCallback(EStatType.MaxMana, OnMaxManaChanged, OnMaxManaChanged);
    }

    private void OnMaxHungerChanged(EStatType type, StatModifier modifier)
    {
        float oldMax = MaxHunger;
        MaxHunger = _stat.GetStat(EStatType.MaxHunger);
        float ratio = MaxHunger > 0f && oldMax > 0f ? MaxHunger / oldMax : 1f;
        CurrentHunger *= ratio;
        CurrentHunger = Mathf.Clamp(CurrentHunger, 0, MaxHunger);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }

    private void OnMaxManaChanged(EStatType type, StatModifier modifier)
    {
        float oldMax = MaxMana;
        MaxMana = _stat.GetStat(EStatType.MaxMana);
        float ratio = MaxMana > 0f && oldMax > 0f ? MaxMana / oldMax : 1f;
        CurrentMana *= ratio;
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }
    public void ConsumeHunger(float amount)
    {
        CurrentHunger = Mathf.Max(CurrentHunger - amount, 0f);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }

    public void RestoreHunger(float amount)
    {
        CurrentHunger = Mathf.Min(CurrentHunger + amount, MaxHunger);
        OnHungerChanged?.Invoke(CurrentHunger, MaxHunger);
    }


    public void RestoreMana(float amount)
    {
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