using System.Collections.Generic;

public class UseItem
{
    public readonly ItemData _itemData;
    public readonly List<IUseItemEffect> _effects;

    public UseItem(ItemData itemData, List<IUseItemEffect> effects)
    {
        _itemData = itemData;
        _effects = new List<IUseItemEffect>(effects);
    }
}