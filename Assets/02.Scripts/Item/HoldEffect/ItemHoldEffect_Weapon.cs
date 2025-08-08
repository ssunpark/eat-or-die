using UnityEngine;

public class ItemHoldEffect_Weapon : IItemHoldEffect
{
    private const string EFFECT_SOURCE = "Weapon";
    private readonly float _meleeDamage;
    private readonly float _magicDamage;
    private readonly float _attackSpeed;
    private readonly float _range;

    public ItemHoldEffect_Weapon(float meleeDamage, float magicDamage, float attackSpeed, float range)
    {
        _meleeDamage = meleeDamage;
        _magicDamage = magicDamage;
        _attackSpeed = attackSpeed;
        _range = range;
    }

    public void Hold(GameObject target)
    {
        target.GetComponent<Player>().Stat.ApplyModifier(EStatType.MeleeDamage, new StatModifier(EStatModifierType.Add, _meleeDamage, EFFECT_SOURCE));
        target.GetComponent<Player>().Stat.ApplyModifier(EStatType.MagicDamage, new StatModifier(EStatModifierType.Add, _magicDamage, EFFECT_SOURCE));
        target.GetComponent<Player>().Stat.ApplyModifier(EStatType.AttackSpeed, new StatModifier(EStatModifierType.Add, _attackSpeed, EFFECT_SOURCE));
        target.GetComponent<Player>().Stat.ApplyModifier(EStatType.AttackRange, new StatModifier(EStatModifierType.Add, _range, EFFECT_SOURCE));
    }

    public void UnHold(GameObject target)
    {
        target.GetComponent<Player>().Stat.RemoveModifiersFrom(EFFECT_SOURCE);
    }
}