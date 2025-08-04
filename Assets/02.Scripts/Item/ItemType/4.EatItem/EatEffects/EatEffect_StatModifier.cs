using UnityEngine;

public class EatEffect_StatModifier : IUseEffect
{
    private const string Food = "Food";
    
    private readonly float _value;
    private readonly float _duration;
    private readonly EStatType _statType;
    private readonly EStatModifierType _modifierType;

    public EatEffect_StatModifier(EStatType statType, float value, float duration, EStatModifierType modifierType)
    {
        _value = value;
        _duration = duration;
        _statType = statType;
        _modifierType = modifierType;
    }
    
    public void Use(GameObject target)
    {
        target.GetComponent<Player>().Stat.ApplyModifier(_statType, new StatModifier(_modifierType, _value, Food,true, _duration));
    }
}