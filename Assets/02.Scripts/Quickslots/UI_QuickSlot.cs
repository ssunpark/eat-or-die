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
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			QuickSlotManager.Instance.OnClickMouseLeft(SlotIndex);   
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			QuickSlotManager.Instance.OnClickMouseRight(SlotIndex);
		}
	}

	public void UpdateSlotUI()
	{
		ItemInstance itemInstanceInSlot = QuickSlotManager.Instance.GetItemInSlot(SlotIndex);
		if (itemInstanceInSlot == null)
		{
			IconImage.gameObject.SetActive(false);
			QuantityText.gameObject.SetActive(false);
			return;
		}
        
		IconImage.sprite = ItemManager.Instance.GetItem(itemInstanceInSlot.ID).ItemDefinition.Icon;
		QuantityText.text = itemInstanceInSlot.Quantity.ToString();
		IconImage.gameObject.SetActive(true);
		QuantityText.gameObject.SetActive(itemInstanceInSlot.Quantity > 1);
	}	
}
