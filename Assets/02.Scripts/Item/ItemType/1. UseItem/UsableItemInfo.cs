using UnityEngine;

public class UsableItemInfo : AItemInfo, IUsable
{
    private readonly IUseAction _useAction;
    private readonly string _interactionTag;

    public string InteractionTag => _interactionTag;
    
    public UsableItemInfo(ItemData itemData, string interactionTag, IUseAction useAction) : base(itemData)
    {
        _interactionTag = interactionTag;
        _useAction = useAction;
    }

    public void Use(GameObject target)
    {
        // target에 도구 사용
        _useAction.UseTool(target);
    }
}