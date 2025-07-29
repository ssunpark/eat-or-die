using System.Collections.Generic;
using UnityEngine;

public class EatItemInfo : AItemInfo, IEatable, IUsable
{
    private readonly List<IEatItemEffect> _effectList;
    
    public string InteractionTag => "Player";

    public EatItemInfo(ItemData itemData, Transform poolParent, List<IEatItemEffect> effectList) : base(itemData, poolParent)
    {
        _effectList = effectList;
        foreach (var effect in effectList)
        {
            ItemData.AddDescription(effect.Description);
        }
    }

    public void Eat(GameObject target)
    {
        foreach (var effect in _effectList)
        {
            effect.UseEffect(target);
        }
    }

    public void Use(GameObject target)
    {
        foreach (var effect in _effectList)
        {
            effect.UseEffect(target);
        }
    }
}