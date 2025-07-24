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
        IconImage.gameObject.SetActive(false);
        QuantityText.gameObject.SetActive(false);
        InventoryManager.Instance.OnSlotUpdated[SlotIndex] += UpdateSlotUI;
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

    public void UpdateSlotUI()
    {
        Item itemInSlot = InventoryManager.Instance.Inventory.SlotList[SlotIndex].Item;
        if (itemInSlot == null)
        {
            IconImage.gameObject.SetActive(false);
            QuantityText.gameObject.SetActive(false);
            return;
        }
        
        IconImage.sprite = ItemManager.Instance.GetItem(itemInSlot.ID).ItemData.Icon;
        QuantityText.text = itemInSlot.Quantity.ToString();
        IconImage.gameObject.SetActive(true);
        QuantityText.gameObject.SetActive(itemInSlot.Quantity > 1);
    }
}
