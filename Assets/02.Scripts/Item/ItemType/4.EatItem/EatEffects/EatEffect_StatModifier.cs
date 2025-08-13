using UnityEngine;

public class EatEffect_StatModifier : IUseEffect, ISkillModifiable
{
    private const string Food = "Food";
    
    private readonly float _value;
    private readonly float _duration;
    private readonly EStatType _statType;
    private readonly EStatModifierType _modifierType;
    public float MultiplyValue { get; set; }

    public EatEffect_StatModifier(EStatType statType, float value, float duration, EStatModifierType modifierType)
    {
        _value = value;
        _duration = duration;
        _statType = statType;
        _modifierType = modifierType;
    }
    
    public void Use(GameObject target)
    {
        Debug.Log($"{_statType}이 {_value * MultiplyValue}만큼 {_modifierType}연산으로 {_duration}초간 증가합니다.");
        target.GetComponent<Player>().Stat.ApplyModifier(_statType, new StatModifier(_modifierType, _value * MultiplyValue, Food,true, _duration));
        MultiplyValue = 1f;
    }
}