using UnityEngine;

// 사용 아이템 효과: {_duration}초 동안 1초마다 마나 +{_value}
public class EatEffect_ManaTimeRecovery : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;

    public EatEffect_ManaTimeRecovery(float value, float duration)
    {
        _value = value;
        _duration = duration;
    }
    
    public void UseEffect()
    {
        Debug.Log($"{_duration}초 동안 1초마다 마나 +{_value}");
    }
}