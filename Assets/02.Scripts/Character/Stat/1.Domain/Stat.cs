using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Stat
{
    public float BaseStat { get; private set; }
    public float CurrentValue { get; private set; }


    private readonly List<StatModifier> _modifiers = new();


    public void SetBaseStat(float value)
    {
        BaseStat = value;
        CurrentValue = TotalStat;
    }
    public Stat(float baseStat)
    {
        BaseStat = baseStat;
    }

    public void AddModifier(StatModifier modifier)
    {
        if (_modifiers.Any(m => m.Source == modifier.Source && m.Type == modifier.Type))
        {
            // 이미 같은 소스와 타입의 모디파이어가 있다면 업데이트
            var existingModifier = _modifiers.First(m => m.Source == modifier.Source && m.Type == modifier.Type);
            existingModifier.Value= modifier.Value; // 값 업데이트
            existingModifier.Duration = modifier.Duration;// 지속시간 업데이트
            Debug.Log($"Updated modifier: {existingModifier.Source}, Type: {existingModifier.Type}, Value: {existingModifier.Value}, Duration: {existingModifier.Duration}");
        }
        else
        {
            // 새로운 모디파이어 추가
            _modifiers.Add(modifier);
            Debug.Log($"Added modifier: {modifier.Source}, Type: {modifier.Type}, Value: {modifier.Value}, Duration: {modifier.Duration}");
        }
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

    public void UpdateModifiers(float deltaTime)
    {
        if(deltaTime <= 0f)
        {
            Debug.LogWarning("Delta time must be greater than zero to update modifiers.");
            return;
        }
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            var mod = _modifiers[i];
            if (mod.IsBuff)
            {
                mod.Duration -= deltaTime;
                if (mod.Duration <= 0f)
                {
                    _modifiers.RemoveAt(i);
                    Debug.Log($"Removed expired modifier: {mod.Source}, Type: {mod.Type}");
                }
            }
        }
    }
}