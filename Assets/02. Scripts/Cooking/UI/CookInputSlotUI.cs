using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//수현
public class CookInputSlotUI : MonoBehaviour, IPointerClickHandler
{
    // 인벤토리 -> 요리 제작 슬롯으로 이동
    // 슬롯에 클릭 이벤트가 발생하면 CookingPanelManager에서 데이터를 저장해주는 메서드 호출
    // 요리 제작 슬롯 -> 인벤토리로 이동
    public int SlotIndex;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭되는 중");
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (HandEntity.Instance.IsHandEmpty)
        {
            var item = CookingPanelManager.Instance.GetItem(SlotIndex);
            if (item != null)
            {
                CookingPanelManager.Instance.RemoveItem(SlotIndex);
                HandEntity.Instance.PickUpItem(item);
                Debug.Log("아이템 줍기");
            }
        }
        else
        {
            if (CookingPanelManager.Instance.CanPlaceItem(SlotIndex, HandEntity.Instance.ItemStack))
            {
                CookingPanelManager.Instance.PlaceItem(SlotIndex, HandEntity.Instance.ItemStack);
                HandEntity.Instance.DropItem();
                Debug.Log("아이템 드롭하기");
            }
        }
    }
}
