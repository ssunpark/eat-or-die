using UnityEngine;

// 사용 아이템 효과: {1}초 동안 1초마다 배고픔이 {0} 증가 합니다.
public class EatEffect_HungerTimeRecovery : IEatItemEffect
{
    private readonly float _value;
    private readonly float _duration;
    public string Description => $"{_duration}초 동안 1초마다 배고픔이 {_value} 증가 합니다.";

    public EatEffect_HungerTimeRecovery(float value, float duration)
    {
        _value = value;
        _duration = duration;
    }
    
    public void UseEffect()
    {
        Debug.Log($"{_duration}초 동안 1초마다 배고픔이 {_value} 증가 합니다.");
    }
}