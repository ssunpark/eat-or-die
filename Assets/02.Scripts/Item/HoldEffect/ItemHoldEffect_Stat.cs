using UnityEngine;

public class ItemHoldEffect_Stat : IItemHoldEffect
{
    private readonly object _source;
    private readonly float _value;
    private readonly EStatType _statType;
    private readonly EStatModifierType _modifierType;
    
    public ItemHoldEffect_Stat(object source, float value, EStatType statType, EStatModifierType modifierType = EStatModifierType.Add)
    {
        _source = source;
        _value = value;
        _statType = statType;
        _modifierType = modifierType;
    }
    
    public void Hold(GameObject target)
    {
        if (!target.GetComponent<Player>().HasStateAuthority && !target.GetComponent<Player>().HasInputAuthority)
            return;
        target.GetComponent<Player>().Stat.ApplyModifier(_statType, new StatModifier(_modifierType, _value, _source));
    }

    public void UnHold(GameObject target)
    {
        if (!target.GetComponent<Player>().HasStateAuthority && !target.GetComponent<Player>().HasInputAuthority)
            return;
        target.GetComponent<Player>().Stat.RemoveModifiersFrom(_source);
    }
}