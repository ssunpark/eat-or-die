using UnityEngine;

public class EatEffect_HungerTimeRecovery : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    private string _description;
    public string Description => _description;

    public EatEffect_HungerTimeRecovery(float value, float duration, string description)
    {
        _value = value;
        _duration = duration;
        _description = EatEffectUtils.FormatSmart(description, _value, _duration);
    }
    
    public void UseEffect()
    {
        Debug.Log(Description);
    }
}