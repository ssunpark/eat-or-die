using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;
    
    public void Initialize(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryManager.Instance.OnClickMouseLeft(SlotIndex);   
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            InventoryManager.Instance.OnClickMouseRight(SlotIndex);
        }
    }
}
