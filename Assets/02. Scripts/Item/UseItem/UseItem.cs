using System.Collections.Generic;

public class UseItem : AItem, IUseable
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
}