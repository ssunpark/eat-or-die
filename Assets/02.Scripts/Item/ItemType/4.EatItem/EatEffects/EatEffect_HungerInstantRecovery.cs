using UnityEngine;

public class EatEffect_HungerInstantRecovery : IUseEffect
{
    private readonly float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_HungerInstantRecovery(float value)
    {
        _value = value;
        _description = $"배고픔이 {_value}만큼 즉시 회복됩니다.";
    }

    public void Use(GameObject target)
    {
        // 매개 변수로 받은 특정 타겟에 대해 효과 적용
        Debug.Log(Description);
    }
}