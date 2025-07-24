using UnityEngine;

public class EatEffect_MagicDamage : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_MagicDamage(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = description;
    }
    
    public void UseEffect(GameObject target)
    {
        Debug.Log(Description);
        // target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.MagicDamage, new StatModifier(EStatModifierType.Add, _value, FoodModifierSource.Food,true, _duration));
    }
}