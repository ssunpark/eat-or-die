using UnityEngine;

public class EatEffect_ManaTimeRecovery : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_ManaTimeRecovery(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = string.Format(description, _value, _duration);
    }

    public void UseEffect()
    {
        Debug.Log(Description);
    }
}