public class ToolItem : AItem, IUseable
{
    private readonly IToolAction ToolAction;
    
    public ToolItem(ItemData itemData, IToolAction toolAction) : base(itemData)
    {
        ToolAction = toolAction;
    }

    public void Use()
    {
        // 도구 사용
        ToolAction.UseTool();
    }
}