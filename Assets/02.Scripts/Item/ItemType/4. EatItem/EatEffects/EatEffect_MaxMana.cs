using UnityEngine;

// 사용 아이템 효과: {_duration}분 동안 최대 마나 +{_value}
public class EatEffect_MaxMana : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    public string Description => $"{_duration}분 동안 최대 마나 +{_value}";

    public EatEffect_MaxMana(float value, float duration)
    {
        _value = value;
        _duration = duration;
    }
    
    public void UseEffect()
    {
        Debug.Log($"{_duration}분 동안 최대 마나 +{_value}");
    }
}