using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookOutputSlotUI : MonoBehaviour, IPointerDownHandler
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;

    private void Start()
    {
        IconImage.gameObject.SetActive(false);
        QuantityText.gameObject.SetActive(false);

        CookingManager.Instance.OnCookOutputUpdated += UpdateSlotUI;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            TakeOutItem();
        }
    }

    // 결과 슬롯의 아이콘과 수량 갱신
    private void TakeOutItem()
    {
        var foodInventory = CookingManager.Instance.FoodInventory;
        var slot = foodInventory.SlotList[SlotIndex];
        if (slot.IsEmpty) return;

        if (HandEntity.Instance.IsHandEmpty)
        {
            // 한 개만 손으로 집기
            HandEntity.Instance.PickUpItem(foodInventory.PopSingleItemInSlot(SlotIndex));
        }
        else
        {
            if (HandEntity.Instance.Item.ID == slot.Item.ID)
            {
                // 스택 합치기
                Item popped = foodInventory.PopSingleItemInSlot(SlotIndex);
                if (!HandEntity.Instance.TryAddItem(popped))
                {
                    // 손에 더 못 넣으면 다시 인벤토리에 넣기
                    slot.AddItem(popped);
                }
            }
        }

        UpdateSlotUI();
    }


    public void UpdateSlotUI()
    {
        var itemInSlot = CookingManager.Instance.FoodInventory.SlotList[SlotIndex].Item;
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