using UnityEngine;

public class EatEffect_BossDamage : IEatItemEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_BossDamage(float value, string description)
    {
        _value = value;
        _description = description;
    }

    public void UseEffect()
    {
        Debug.Log(Description);
    }
}