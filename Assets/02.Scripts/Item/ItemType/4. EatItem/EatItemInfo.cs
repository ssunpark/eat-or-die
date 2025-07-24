using System.Collections.Generic;
using UnityEngine;

public class EatItemInfo : AItemInfo, IEatable, IUsable
{
    private readonly List<IEatItemEffect> _effectList;

    public EatItemInfo(ItemData itemData, List<IEatItemEffect> effectList) : base(itemData)
    {
        _effectList = effectList;
        foreach (var effect in effectList)
        {
            ItemData.AddDescription(effect.Description);
        }
    }

    public void Eat()
    {
        foreach (var effect in _effectList)
        {
            effect.UseEffect();
        }
    }

    public void Use(GameObject target)
    {
        // 타겟에게 효과 주도록 수정
        foreach (var effect in _effectList)
        {
            effect.UseEffect();
        }
    }
}