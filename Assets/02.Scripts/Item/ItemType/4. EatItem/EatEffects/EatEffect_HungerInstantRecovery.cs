using System;
using UnityEngine;

// 사용 아이템 효과: 배고픔이 Value 증가 합니다.
public class EatEffect_HungerInstantRecovery : IEatItemEffect
{
    private readonly float _value;
    public string Description => $"배고픔이 {_value} 증가 합니다.";

    public EatEffect_HungerInstantRecovery(float value)
    {
        _value = value;
    }

    public void UseEffect()
    {
        // 매개 변수로 받은 특정 타겟에 대해 효과 적용
        Debug.Log($"배고픔이 {_value} 증가 합니다.");
    }
}