using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StorageSlot : MonoBehaviour, IPointerDownHandler
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;
    
    public void Initialize(int slotIndex)
    {
        SlotIndex = slotIndex;
        IconImage.gameObject.SetActive(false);
        QuantityText.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SharedStorageManager.Instance.OnClickMouseLeft(SlotIndex);   
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SharedStorageManager.Instance.OnClickMouseRight(SlotIndex);
        }
    }

    public void UpdateSlotUI()
    {
        NetworkedItem itemInstanceInStorage = SharedStorageManager.Instance.GetItemInSlot(SlotIndex);
        
        if (itemInstanceInStorage.ID == 0)
        {
            IconImage.gameObject.SetActive(false);
            QuantityText.gameObject.SetActive(false);
            return;
        }

        ItemInstance itemInstanceInSlot = new ItemInstance(ItemManager.Instance.GetItem(
                itemInstanceInStorage.ID),
                itemInstanceInStorage.Quantity,
                itemInstanceInStorage.Durability);
        
        IconImage.sprite = ItemManager.Instance.GetItem(itemInstanceInSlot.ID).ItemDefinition.Icon;
        QuantityText.text = itemInstanceInSlot.Quantity.ToString();
        IconImage.gameObject.SetActive(true);
        QuantityText.gameObject.SetActive(itemInstanceInSlot.Quantity > 1);
    } 
}