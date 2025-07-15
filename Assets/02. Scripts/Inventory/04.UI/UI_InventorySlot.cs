using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : MonoBehaviour,
    IPointerClickHandler, 
    IBeginDragHandler, 
    IDragHandler,
    IEndDragHandler,
    IItemDropHandler
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;
    
    public void Initialize(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        Debug.Log($"{eventData.pointerDrag.name}");
        // 아이콘 이미지와 수량 텍스트 숨기기
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        IconImage.gameObject.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out IItemDropHandler dropHandler))
        {
            SwapItems(eventData.pointerCurrentRaycast.gameObject.GetComponent<IItemDropHandler>(), dropHandler);
        }
    }
    
    public ItemStack GetItemStack()
    {
        return InventoryManager.Instance.GetItemStack(SlotIndex);
    }
    
    public bool CanPutItem(ItemStack itemStack)
    {
        return InventoryManager.Instance.TryPutItem(SlotIndex, itemStack);
    }
    
    public void SwapItems(IItemDropHandler from, IItemDropHandler to)
    {
        if (from == to)
        {
            return;
        }
    }
}
