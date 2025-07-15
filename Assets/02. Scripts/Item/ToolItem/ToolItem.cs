using UnityEngine;

public class ToolItem : AItem, IInteractable
{
    private readonly IToolAction ToolAction;
    
    public ToolItem(ItemData itemData, IToolAction toolAction) : base(itemData)
    {
        ToolAction = toolAction;
    }

    public void Interact(GameObject target)
    {
        // 도구 사용
        ToolAction.UseTool();
    }
}