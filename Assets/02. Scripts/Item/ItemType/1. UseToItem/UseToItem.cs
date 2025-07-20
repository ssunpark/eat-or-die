using UnityEngine;

public class UseToItem : AItem, IUseTo, IEquipable
{
    private readonly IUseToAction _useToAction;
    public readonly string _interactionTag;
    
    public UseToItem(ItemData itemData, string interactionTag, IUseToAction useToAction) : base(itemData)
    {
        _interactionTag = interactionTag;
        _useToAction = useToAction;
    }

    public void UseTo(GameObject target)
    {
        // target에 도구 사용
        _useToAction.UseTool(target);
    }

    public void Equip()
    {
        // 장착하면 상호작용 할 태그 수정
    }

    public void Unequip()
    {
        // 해제하면 상호작용 할 태그 수정
    }
}