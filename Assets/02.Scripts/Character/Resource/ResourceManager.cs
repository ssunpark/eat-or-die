using System;
using UnityEngine;
public class ResourceManager
{
    private readonly StatManager _stat;

    public float CurrentSatiety { get; private set; }

    public event Action<float, float> OnSatietyChanged;

    public ResourceManager(StatManager stat)
    {
        _stat = stat;
        CurrentSatiety = _stat.GetStat(EStatType.MaxSatiety);
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

    public void ResetAll()
    {
        SetSatiety(_stat.GetStat(EStatType.MaxSatiety));
    }


    public void SetSatiety(float value)
    {
        float max = _stat.GetStat(EStatType.MaxSatiety);
        CurrentSatiety = Mathf.Clamp(value, 0f, max);
        OnSatietyChanged?.Invoke(CurrentSatiety, max);
    }
}