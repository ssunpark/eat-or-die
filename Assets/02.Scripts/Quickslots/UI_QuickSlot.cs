using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuickSlot : MonoBehaviour, IPointerDownHandler
{
	public int SlotIndex;
	public Image IconImage;
	public TextMeshProUGUI QuantityText;
    
	public void Initialize(int slotIndex)
	{
		SlotIndex = slotIndex;
		IconImage.gameObject.SetActive(false);
		QuantityText.gameObject.SetActive(false);
		QuickSlotManager.Instance.OnQuickSlotUpdated[SlotIndex] += UpdateSlotUI;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			QuickSlotManager.Instance.OnSelectSlot(SlotIndex);   
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			QuickSlotManager.Instance.OnClickMouseRight(SlotIndex);
		}
	}

	public void UpdateSlotUI()
	{
		Item itemInSlot = QuickSlotManager.Instance.QuickSlots.SlotList[SlotIndex].Item;
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
