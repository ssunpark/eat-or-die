using UnityEngine;
using UnityEngine.EventSystems;
public class CookingInputSlot : MonoBehaviour, IDropHandler
{
    public IngredientItemHandler CurrentItem;

    public void OnDrop(PointerEventData eventData)
    {
        IngredientItemHandler draggedItem = eventData.pointerDrag.GetComponent<IngredientItemHandler>();
        if (draggedItem != null && CurrentItem == null)
        {
            CurrentItem = draggedItem;
            draggedItem.transform.SetParent(transform);
            Debug.Log($"재료 {draggedItem.data.Name} 슬롯에 등록됨");
        }
    }
}
