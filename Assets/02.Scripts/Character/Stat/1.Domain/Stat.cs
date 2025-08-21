using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Stat
{
    public event Action<EStatType, StatModifier> ModifierAdded;
    public event Action<EStatType, StatModifier> ModifierRemoved;
    public event Action<EStatType> BaseChanged;
    public float BaseStat { get; private set; }
    public float CurrentValue { get; private set; }
    private EStatType _statType;

    private readonly List<StatModifier> _modifiers = new();


    public void SetBaseStat(float value)
    {
        BaseStat = value;
        CurrentValue = TotalStat;
        BaseChanged?.Invoke(_statType);
    }
    public Stat(EStatType statType, float baseStat)
    {
        _statType = statType;
        BaseStat = baseStat;
        CurrentValue = TotalStat;
    }

    public void AddModifier(StatModifier modifier)
    {
        if (_modifiers.Any(m => m.Source.ToString() == modifier.Source.ToString() && m.Type == modifier.Type))
        {
            var existingModifier = _modifiers.First(m => m.Source.ToString() == modifier.Source.ToString() && m.Type == modifier.Type);
           existingModifier.Value = modifier.Value;
            existingModifier.Duration = modifier.Duration;
        }
        else
        {
            _modifiers.Add(modifier);
            ModifierAdded?.Invoke(_statType, modifier);
        }
    }

    public void RemoveModifiersFrom(object source)
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].Source.Equals(source))
            {
                ModifierRemoved?.Invoke(_statType, _modifiers[i]);
                _modifiers.RemoveAt(i);
            }
        }
    }

    public void UpdateModifiers(float deltaTime)
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            var mod = _modifiers[i];
            if (mod.IsBuff)
            {
                mod.Duration -= deltaTime;
                if (mod.Duration <= 0f)
                {
                    ModifierRemoved?.Invoke(_statType, mod);
                    _modifiers.RemoveAt(i);
                }
            }
        }
    }

    public void ClearAllModifiers()
    {
        _modifiers.Clear();
    }

    public float TotalStat => CalculateFinalStat();

    private float CalculateFinalStat()
    {
        float baseValue = BaseStat;

        float addSum = 0f;
        float multiplyProduct = 1f;

        foreach (var mod in _modifiers)
        {
            switch (mod.Type)
            {
                case EStatModifierType.Add:
                    addSum += mod.Value;
                    break;
                case EStatModifierType.Percentage:
                    Debug.LogError("EStatModifierType.Percentage 는 쓰이지 않습니다. CSV의 Percentage -> Multiply로 바꿔주세요");
                    break;
                case EStatModifierType.Multiply:
                    multiplyProduct += mod.Value;
                    break;
            }
        }

        float result = (baseValue * multiplyProduct) + addSum;
        return result;
    }

    
}