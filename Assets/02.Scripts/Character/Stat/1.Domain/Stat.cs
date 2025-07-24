using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Stat
{
    public int Level;
    public int Exp;
    public int ExpToNextLevel => (Level + 1) * 100; // 예시
    public bool CanLevelUp;
    public float BaseStat { get; private set; }
    public float CurrentValue { get; private set; }

    private float _increasePerGap;
    private int _increaseGap;

    private readonly List<StatModifier> _modifiers = new();


    public void SetBaseStat(float value)
    {
        BaseStat = value;
        CurrentValue = TotalStat;
    }
    public Stat(float baseStat)
    {
        Level = 0;
        BaseStat = baseStat;
        CanLevelUp = false;
        _increasePerGap = 0f;
        _increaseGap = 1;
    }

    public Stat(float baseStat, bool canLevelUp, float increasePerGap, int increaseGap = 1)
    {
        Level = 0;
        BaseStat = baseStat;
        CanLevelUp = canLevelUp;
        _increasePerGap = increasePerGap;
        _increaseGap = increaseGap;
    }

    public void SetCurrent(float value)
    {
        CurrentValue = Mathf.Clamp(value, 0f, TotalStat);
    }

    public void Restore(float amount) => SetCurrent(CurrentValue + amount);
    public void Consume(float amount) => SetCurrent(CurrentValue - amount);
    public void LevelUp()
    {
        if (!CanLevelUp) return;
        if (Exp < ExpToNextLevel) return;
        Exp -= ExpToNextLevel;
        Level++;
    }

    public void SetLevel(int level)
    {
        if (level < 0) return;
        Level = level;
        CurrentValue = TotalStat;
    }

    public void AddModifier(StatModifier modifier)
    {
        _modifiers.Add(modifier);
    }

    public void RemoveModifiersFrom(object source)
    {
        _modifiers.RemoveAll(mod => mod.Source == source);
    }

    public void ClearAllModifiers()
    {
        _modifiers.Clear();
    }

    public float TotalStat => CalculateFinalStat();

    private float CalculateFinalStat()
    {
        float baseValue = BaseStat + (Level / _increaseGap) * _increasePerGap;

        float addSum = 0f;
        float percentageSum = 0f;
        float multiplyProduct = 1f;

        foreach (var mod in _modifiers)
        {
            switch (mod.Type)
            {
                case EStatModifierType.Add:
                    addSum += mod.Value;
                    break;
                case EStatModifierType.Percentage:
                    percentageSum += mod.Value;
                    break;
                case EStatModifierType.Multiply:
                    multiplyProduct *= mod.Value;
                    break;
            }
        }

        float result = (baseValue * multiplyProduct) + (baseValue * percentageSum) + addSum;
        return result;
    }
}