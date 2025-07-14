using System;
using UnityEngine;

// 사용 아이템 효과: 즉시 배고픔 회복
public class UseItemEffect_Hungry : IUseItemEffect
{
    public float Value { get; set; }

    public UseItemEffect_Hungry(float value)
    {
        if (value <= 0f)
        {
            throw new Exception("사용 아이템의 수치가 0이하일 수 없습니다.");
        }
        
        Value = value;
    }

    public void UseEffect()
    {
        // 매개 변수로 받은 특정 타겟에 대해 효과 적용
        Debug.Log("배고픔 채우기 아이템 사용입니다. (Test)");
    }
}