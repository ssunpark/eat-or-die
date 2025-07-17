using UnityEngine;

public class InteractableItem : AItem, IInteractable
{
    private readonly IInteractableAction _interactableAction;
    
    public InteractableItem(ItemData itemData, IInteractableAction interactableAction) : base(itemData)
    {
        _interactableAction = interactableAction;
    }

    public void Interact(GameObject target)
    {
        // 도구 사용
        _interactableAction.UseTool();
    }
}