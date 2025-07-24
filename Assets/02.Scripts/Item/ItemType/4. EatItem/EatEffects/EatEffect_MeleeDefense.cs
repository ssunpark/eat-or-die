using UnityEngine;

public class EatEffect_MeleeDefense : IEatItemEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_MeleeDefense(float value, string description)
    {
        _value = value;
        _description = description;
    }

    public void UseEffect()
    {
        Debug.Log(Description);
    }
}