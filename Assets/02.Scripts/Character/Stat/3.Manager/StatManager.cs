using System;
using System.Collections.Generic;
using UnityEngine;

public class StatManager
{
    private readonly Dictionary<EStatType, Stat> _stats = new();

    // 🔥 이벤트 시스템
    public event Action<EStatType, StatModifier> OnModifierAdded;
    public event Action<EStatType, StatModifier> OnModifierRemoved;

    public StatManager(IStatDataRepository statRepo)
    {
        foreach (var data in statRepo.GetCharacterStatData())
        {
            _stats[data.StatType] = new Stat(data.StatType, data.BaseAmount);

            // 각 Stat의 내부 이벤트 → StatManager 이벤트로 중계
            _stats[data.StatType].ModifierAdded += (type, mod) => OnModifierAdded?.Invoke(type, mod);
            _stats[data.StatType].ModifierRemoved += (type, mod) => OnModifierRemoved?.Invoke(type, mod);
        }
    }

    public float GetStat(EStatType type)
    {
        return _stats.TryGetValue(type, out var stat) ? stat.TotalStat : 0f;
    }

    public void ApplyModifier(EStatType type, StatModifier modifier)
    {
        if (_stats.TryGetValue(type, out var stat))
            stat.AddModifier(modifier);
    }

    public void RemoveModifiersFrom(object source)
    {
        foreach (var stat in _stats.Values)
            stat.RemoveModifiersFrom(source);
    }

    public void ClearAllModifiers()
    {
        foreach (var stat in _stats.Values)
            stat.ClearAllModifiers();
    }

    public void UpdateStats(float deltaTime)
    {
        foreach (var stat in _stats.Values)
            stat.UpdateModifiers(deltaTime);
    }

    public Dictionary<EStatType, float> GetStatSnapshot()
    {
        var snapshot = new Dictionary<EStatType, float>();
        foreach (var kvp in _stats)
        {
            snapshot[kvp.Key] = kvp.Value.TotalStat;
        }
        return snapshot;
    }

    public Stat GetStatInstance(EStatType type)
    {
        _stats.TryGetValue(type, out var stat);
        return stat;
    }

    // Modifier 직접 구독 가능하게 해줌
    public void RegisterModifierCallback(
        EStatType type,
        Action<EStatType, StatModifier> onAdd,
        Action<EStatType, StatModifier> onRemove)
    {
        if (_stats.TryGetValue(type, out var stat))
        {
            stat.ModifierAdded += onAdd;
            stat.ModifierRemoved += onRemove;
        }
    }
}
