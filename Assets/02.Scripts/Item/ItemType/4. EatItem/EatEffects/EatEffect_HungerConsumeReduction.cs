using UnityEngine;

// 사용 아이템 효과: {_duration}분동안 달릴 때 음식 소모 {_value}%
public class EatEffect_HungerConsumeReduction : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    public string Description => $"{_duration}분동안 달릴 때 음식 소모 {_value}%";

    public EatEffect_HungerConsumeReduction(float value, float duration)
    {
        _value = value;
        _duration = duration;
    }

    public void UseEffect()
    {
        Debug.Log($"{_duration}분동안 달릴 때 음식 소모 {_value}%");
    }
}