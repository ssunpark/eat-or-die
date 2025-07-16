using System.Collections.Generic;
using UnityEngine;

public class UseItem : AItem, IUseable, IInteractable
{
    private readonly List<IUseItemEffect> _effects;

    public UseItem(ItemData itemData, List<IUseItemEffect> effects) : base(itemData)
    {
        _effects = new List<IUseItemEffect>(effects);
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