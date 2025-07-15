using UnityEngine;
using UnityEngine.EventSystems;
public class CookingInputSlot : MonoBehaviour, IDropHandler
{
    public IngredientItemHandler IngredientItemHandler;

    public void OnDrop(PointerEventData eventData)
    {
        // IngredientItemHandler droppedItem = eventData.
    }
}
