using UnityEngine;

public class EatEffect_StatModifier : IEatItemEffect
{
    private const string Food = "Food";
    
    private readonly float _value;
    private readonly float _duration;
    private readonly EStatType _statType;
    private readonly EStatModifierType _modifierType;
    
    private string _description;
    public string Description => _description;

    public EatEffect_StatModifier(EStatType statType, float value, float duration, EStatModifierType modifierType, string description)
    {
        _value = value;
        _duration = duration;
        _statType = statType;
        _modifierType = modifierType;
        _description = description;
    }
    
    public void UseEffect(GameObject target)
    {
        Debug.Log(Description);
        target.GetComponent<PlayerController>().Stat.ApplyModifier(_statType, new StatModifier(_modifierType, _value, Food,true, _duration));
    }
}