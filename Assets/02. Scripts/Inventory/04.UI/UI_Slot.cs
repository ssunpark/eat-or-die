using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour,
    IPointerClickHandler, 
    IBeginDragHandler, 
    IDragHandler, 
    IEndDragHandler
{
    public int SlotIndex;
    private Image _iconImage;
    private TextMeshProUGUI _quantityText;
    
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }
}
