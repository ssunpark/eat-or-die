using System.Collections.Generic;
using System.Linq;
public class Stat
{
    public int Level;
    public bool CanLevelUp;
    public float BaseStat { get; private set; }

    private float _increasePerGap;
    private int _increaseGap;

    private readonly List<StatModifier> _modifiers = new();

    public void SetBaseStat(float value)
    {
        BaseStat = value;
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

    public void LevelUp()
    {
        if (!CanLevelUp) return;
        Level++;
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
                case StatModifierType.Add:
                    addSum += mod.Value;
                    break;
                case StatModifierType.Percentage:
                    percentageSum += mod.Value;
                    break;
                case StatModifierType.Multiply:
                    multiplyProduct *= mod.Value;
                    break;
            }
        }

        float result = (baseValue * multiplyProduct) + (baseValue * percentageSum) + addSum;
        return result;
    }
}