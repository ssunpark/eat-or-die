using UnityEngine;

public class EatEffect_MeleeDamage : IEatItemEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_MeleeDamage(float value, string description)
    {
        _value = value;
        _description = string.Format(description, _value);
    }
    
    public void UseEffect()
    {
        Debug.Log(Description);
    }
}