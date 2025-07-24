using UnityEngine;

public class EatEffect_HungerConsumeReduction : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_HungerConsumeReduction(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = description;
    }

    public void UseEffect(GameObject target)
    {
        Debug.Log(Description);
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.ConsumptionRate, new StatModifier(EStatModifierType.Add, _value, FoodModifierSource.Food,true, _duration));
    }
}