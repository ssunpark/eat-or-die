using UnityEngine;

public class EatEffect_ManaInstantRecovery : IUseEffect, ISkillModifiable
{
    private readonly float _value;
    private string _description;
    public string Description => _description;
    public float MultiplyValue { get; set; }

    public EatEffect_ManaInstantRecovery(float value)
    {
        _value = value;
        _description = $"마력이 {_value}만큼 즉시 회복됩니다.";
    }

    public void Use(GameObject target)
    {
        target.GetComponent<Player>().Resource.RestoreMana(_value * MultiplyValue);
    }
}