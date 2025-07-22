using UnityEngine;

public class EatEffect_AttackSpeed : IEatItemEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_AttackSpeed(float value, string description)
    {
        _value = value;
        _description = string.Format(description, _value * 100);
    }
    
    public void UseEffect()
    {
        Debug.Log(Description);
    }
}