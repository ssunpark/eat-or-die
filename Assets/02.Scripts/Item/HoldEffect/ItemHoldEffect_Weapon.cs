using UnityEngine;

public class ItemHoldEffect_Weapon : IItemHoldEffect
{
    private const string EFFECT_SOURCE = "Weapon";
    private readonly float _damage;
    private readonly float _attackSpeed;
    private readonly float _range;

    public ItemHoldEffect_Weapon(float damage, float attackSpeed, float range)
    {
        _damage = damage;
        _attackSpeed = attackSpeed;
        _range = range;
    }

    public void Hold(GameObject target)
    {
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.MeleeDamage, new StatModifier(EStatModifierType.Add, _damage, EFFECT_SOURCE));
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.AttackSpeed, new StatModifier(EStatModifierType.Add, _attackSpeed, EFFECT_SOURCE));
        target.GetComponent<PlayerController>().Stat.ApplyModifier(EStatType.AttackRange, new StatModifier(EStatModifierType.Add, _range, EFFECT_SOURCE));
    }

    public void UnHold(GameObject target)
    {
        target.GetComponent<PlayerController>().Stat.RemoveModifiersFrom(EFFECT_SOURCE);
    }
}