using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//수현
public class CookInputSlotUI : MonoBehaviour, IPointerDownHandler
{
    // 인벤토리 -> 요리 제작 슬롯으로 이동
    // 슬롯에 클릭 이벤트가 발생하면 CookingPanelManager에서 데이터를 저장해주는 메서드 호출
    // 요리 제작 슬롯 -> 인벤토리로 이동
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;
    
    public void Start()
    {
        IconImage.gameObject.SetActive(false);
        QuantityText.gameObject.SetActive(false);
        CookingPanelManager.Instance.OnCookingSlotUpdated[SlotIndex] += UpdateSlotUI;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        { 
            CookingPanelManager.Instance.OnClickMouseLeft(SlotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (eventData.pointerEnter.GetComponent<UI_InventorySlot>() != null)
            {
                InventoryManager.Instance.OnClickMouseRight(SlotIndex);
            }else if (eventData.pointerEnter.GetComponent<CookInputSlotUI>() != null)
            {
                CookingPanelManager.Instance.OnClickMouseRight(SlotIndex);
            }
        }
    }
    
    // 슬롯에 있는 아이콘과 수량 표시 갱신
    public void UpdateSlotUI()
    {
        ItemStack itemInSlot = CookingPanelManager.Instance.Inventory.SlotList[SlotIndex].ItemStack;
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
