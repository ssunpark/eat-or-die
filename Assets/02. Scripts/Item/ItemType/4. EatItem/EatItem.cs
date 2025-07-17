using System.Collections.Generic;
using UnityEngine;

public class EatItem : AItem, IUseable, IInteractable
{
    private readonly List<IEatItemEffect> _effects;

    public EatItem(ItemData itemData, List<IEatItemEffect> effects) : base(itemData)
    {
        _effects = new List<IEatItemEffect>(effects);
    }

    public void Use()
    {
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }

    public void Interact(GameObject target)
    {
        // 타겟에게 효과 주도록 수정
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }
}