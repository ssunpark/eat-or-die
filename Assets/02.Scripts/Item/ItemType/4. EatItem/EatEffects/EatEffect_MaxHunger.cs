using UnityEngine;

// 사용 아이템 효과: {_duration}분동안 최대 배고픔 +{_value}
public class EatEffect_MaxHunger : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;

    public EatEffect_MaxHunger(float value, float duration)
    {
        _value = value;
        _duration = duration;
    }
    
    public void UseEffect()
    {
        Debug.Log($"{_duration}분동안 최대 배고픔 +{_value}");
    }
}