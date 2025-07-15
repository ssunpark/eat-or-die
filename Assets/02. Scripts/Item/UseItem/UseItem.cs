using System.Collections.Generic;

public class UseItem : AItem
{
    private readonly List<IUseItemEffect> _effects;

    public UseItem(ItemData itemData, List<IUseItemEffect> effects) : base(itemData)
    {
        _effects = new List<IUseItemEffect>(effects);
    }

    // 일단 같은 이벤트 등록
    public override void OnSlotEvent()
    {
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }

    public override void OnUseEvent()
    {
        foreach (var effect in _effects)
        {
            effect.UseEffect();
        }
    }
}