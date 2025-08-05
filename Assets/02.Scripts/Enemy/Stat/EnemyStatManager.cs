using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatManager
{
    private readonly Dictionary<EStatType, Stat> _stats = new();

    public event Action<EStatType, StatModifier> OnModifierAdded;
    public event Action<EStatType, StatModifier> OnModifierRemoved;

    public EnemyStatManager(int id)
    {
        EnemyRawData enemyData = EnemyDataManager.Instance.EnemyRawDataDictionary[id];
    }

    public float GetStat(EStatType type)
    {
        return _stats.TryGetValue(type, out Stat stat) ? stat.TotalStat : 0f;
    }

    public void ApplyModifier(EStatType type, StatModifier modifier)
    {
        if (_stats.TryGetValue(type, out Stat stat))
            stat.AddModifier(modifier);
    }

    public void RemoveModifiersFrom(object source)
    {
        foreach (Stat stat in _stats.Values)
            stat.RemoveModifiersFrom(source);
    }

    public void ClearAllModifiers()
    {
        foreach (Stat stat in _stats.Values)
            stat.ClearAllModifiers();
    }

    public void UpdateStats(float deltaTime)
    {
        foreach (Stat stat in _stats.Values)
            stat.UpdateModifiers(deltaTime);
    }

    public Dictionary<EStatType, float> GetStatSnapshot()
    {
        Dictionary<EStatType, float> snapshot = new();
        foreach (KeyValuePair<EStatType, Stat> kvp in _stats)
        {
            snapshot[kvp.Key] = kvp.Value.TotalStat;
        }
        return snapshot;
    }

    public Stat GetStatInstance(EStatType type)
    {
        _stats.TryGetValue(type, out Stat stat);
        return stat;
    }

    // Modifier 직접 구독 가능하게 해줌
    public void RegisterModifierCallback(
        EStatType type,
        Action<EStatType, StatModifier> onAdd,
        Action<EStatType, StatModifier> onRemove)
    {
        if (_stats.TryGetValue(type, out Stat stat))
        {
            stat.ModifierAdded += onAdd;
            stat.ModifierRemoved += onRemove;
        }
    }
}
