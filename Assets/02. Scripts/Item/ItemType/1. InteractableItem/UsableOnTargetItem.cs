using UnityEngine;

public class UsableOnTargetItem : AItem, IUsableOnTarget
{
    private readonly IUseToAction _useToAction;
    
    public UsableOnTargetItem(ItemData itemData, IUseToAction useToAction) : base(itemData)
    {
        _useToAction = useToAction;
    }

    public void UseOn(GameObject target)
    {
        // target에 도구 사용
        _useToAction.UseTool(target);
    }
}