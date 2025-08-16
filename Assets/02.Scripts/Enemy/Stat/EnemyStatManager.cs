using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatManager
{
    private readonly Dictionary<EStatType, Stat> _statDictionary = new();

    // public event Action<EStatType, StatModifier> OnModifierAdded;
    // public event Action<EStatType, StatModifier> OnModifierRemoved;

    public EnemyStatManager(int id)
    {
        EnemyRawData enemyData = EnemyDataManager.Instance.EnemyRawDataDictionary[id];
        Debug.Log("attackRange: " + enemyData.AttackRange);
        _statDictionary = new Dictionary<EStatType, Stat>
        {
            { EStatType.EnemyHunger, new Stat(EStatType.EnemyHunger, enemyData.Hunger) },
            { EStatType.EnemyMoveSpeed, new Stat(EStatType.EnemyMoveSpeed, enemyData.MoveSpeed) },
            { EStatType.EnemyDamage, new Stat(EStatType.EnemyDamage, enemyData.Damage) },
            { EStatType.EnemyAttackSpeed, new Stat(EStatType.EnemyAttackSpeed, enemyData.AttackSpeed) },
            { EStatType.EnemyDetectionRange, new Stat(EStatType.EnemyDetectionRange, enemyData.DetectionRange)},
            { EStatType.EnemyTriggerRange, new Stat(EStatType.EnemyTriggerRange, enemyData.TriggerRange)},
            { EStatType.EnemyTriggerAngle, new Stat(EStatType.EnemyTriggerAngle, enemyData.TriggerAngle)},
            { EStatType.EnemyAttackRange, new Stat(EStatType.EnemyAttackRange, enemyData.AttackRange) },
            { EStatType.EnemyAttackAngle, new Stat(EStatType.EnemyAttackAngle, enemyData.AttackAngle) },
            { EStatType.EnemyHitCount, new Stat(EStatType.EnemyHitCount, enemyData.HitCount) },
            { EStatType.EnemyMeleeDefense, new Stat(EStatType.EnemyMeleeDefense, enemyData.MeleeDefense) },
            { EStatType.EnemyMagicDefense, new Stat(EStatType.EnemyMagicDefense, enemyData.MagicDefense) },
        };
    }

    public float GetStat(EStatType type)
    {
        return _statDictionary.TryGetValue(type, out Stat stat) ? stat.TotalStat : 0f;
    }

    public void ApplyModifier(EStatType type, StatModifier modifier)
    {
        if (_statDictionary.TryGetValue(type, out Stat stat))
            stat.AddModifier(modifier);
    }

    public void RemoveModifiersFrom(object source)
    {
        foreach (Stat stat in _statDictionary.Values)
            stat.RemoveModifiersFrom(source);
    }

    public void ClearAllModifiers()
    {
        foreach (Stat stat in _statDictionary.Values)
            stat.ClearAllModifiers();
    }

    public void UpdateStats(float deltaTime)
    {
        foreach (Stat stat in _statDictionary.Values)
            stat.UpdateModifiers(deltaTime);
    }

    public Dictionary<EStatType, float> GetStatSnapshot()
    {
        Dictionary<EStatType, float> snapshot = new();
        foreach (KeyValuePair<EStatType, Stat> kvp in _statDictionary)
        {
            snapshot[kvp.Key] = kvp.Value.TotalStat;
        }
        return snapshot;
    }

    public Stat GetStatInstance(EStatType type)
    {
        _statDictionary.TryGetValue(type, out Stat stat);
        return stat;
    }

    // Modifier 직접 구독 가능하게 해줌
    public void RegisterModifierCallback(
        EStatType type,
        Action<EStatType, StatModifier> onAdd,
        Action<EStatType, StatModifier> onRemove)
    {
        if (_statDictionary.TryGetValue(type, out Stat stat))
        {
            stat.ModifierAdded += onAdd;
            stat.ModifierRemoved += onRemove;
        }
    }
}
