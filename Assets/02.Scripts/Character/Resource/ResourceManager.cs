using System;
using UnityEngine;

// Todo: DDD로 변경
public class ResourceManager
{
    private readonly StatManager _stat;

    public float CurrentSatiety { get; private set; }
    public float CurrentMana { get; private set; }

    public event Action<float, float> OnSatietyChanged;
    public event Action<float, float> OnManaChanged;

    public ResourceManager(StatManager stat)
    {
        _stat = stat;
        CurrentSatiety = _stat.GetStat(EStatType.MaxHunger);
    }
    public void ConsumeSatiety(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxHunger);
        CurrentSatiety = Mathf.Max(CurrentSatiety - amount, 0f);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }

    public void RestoreSatiety(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxHunger);
        CurrentSatiety = Mathf.Min(CurrentSatiety + amount, max);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }


    public void RestoreMana(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxMana);
        CurrentMana = Mathf.Min(CurrentMana + amount, max);
        OnManaChanged?.Invoke(CurrentMana, max);
    }
    public void ConsumeMana(float amount)
    {
        float max = _stat.GetStat(EStatType.MaxMana);
        CurrentMana = Mathf.Max(CurrentMana - amount, 0f);
        OnManaChanged?.Invoke(CurrentMana, max);
    }

    public bool HasEnoughMana(float amount)
    {
        return CurrentMana >= amount;
    }

    public void ResetAll()
    {
        SetSatiety(_stat.GetStat(EStatType.MaxHunger));
        SetMana(_stat.GetStat(EStatType.MaxMana));
    }

    public void SetMana(float value)
    {
        float max = _stat.GetStat(EStatType.MaxMana);
        CurrentMana = Mathf.Clamp(value, 0f, max);
        OnManaChanged?.Invoke(CurrentMana, max);
    }
    public void SetSatiety(float value)
    {
        float max = _stat.GetStat(EStatType.MaxHunger);
        CurrentSatiety = Mathf.Clamp(value, 0f, max);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }
}