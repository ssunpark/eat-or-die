using UnityEngine;

public class EatEffect_HungerInstantRecovery : IUseEffect
{
    public float Value => _value;
    private float _value;
    private string _description;
    public string Description => _description;

    public EatEffect_HungerInstantRecovery(float value)
    {
        _value = value;
        _description = $"배고픔이 {_value}만큼 즉시 회복됩니다.";
    }

    public void Use(GameObject target)
    {
        Debug.Log($"배고픔이 {_value}만큼 즉시 회복됩니다.");
        target.GetComponent<Player>().TryHealOrDamageFromEat(_value);
    }

    public void Use(GameObject target, float extraValue)
    {
        Debug.Log($"배고픔이 {_value + extraValue}만큼 즉시 회복됩니다.");
        target.GetComponent<Player>().TryHealOrDamageFromEat(_value + extraValue);
    }
}