using UnityEngine;

public class EatEffect_Defense : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_Defense(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = description;
    }

    public void UseEffect(GameObject target)
    {
        Debug.Log(Description);
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.Armor, new StatModifier(EStatModifierType.Add, _value, FoodModifierSource.Food,true, _duration));
    }
}