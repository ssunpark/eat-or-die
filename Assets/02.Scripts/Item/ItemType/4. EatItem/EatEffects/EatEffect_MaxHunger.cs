using UnityEngine;

public class EatEffect_MaxHunger : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_MaxHunger(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = description;
    }

    public void UseEffect(GameObject target)
    {
        Debug.Log(Description);
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.MaxSatiety, new StatModifier(EStatModifierType.Add, _value, FoodModifierSource.Food,true, _duration));
    }
}