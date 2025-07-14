using UnityEngine;

public class UseItemEffect_Hungry : IUseItemEffect
{
    public float Value { get; set; }
    public float Duration { get; set; }

    public UseItemEffect_Hungry(float value, float duration)
    {
        // TODO: 유효성 검사
        Value = value;
        Duration = duration;
    }

    public void UseEffect()
    {
        // 매개 변수로 받은 특정 타겟에 대해 효과 적용
        Debug.Log("배고픔 채우기 아이템 사용입니다. (Test)");
    }
}