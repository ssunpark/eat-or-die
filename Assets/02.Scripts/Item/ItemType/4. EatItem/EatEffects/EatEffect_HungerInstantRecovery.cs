using UnityEngine;

public class EatEffect_HungerInstantRecovery : IEatItemEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_HungerInstantRecovery(float value, string description)
    {
        _value = value;
        _description = description;
    }

    public void UseEffect()
    {
        // 매개 변수로 받은 특정 타겟에 대해 효과 적용
        Debug.Log(Description);
    }
}