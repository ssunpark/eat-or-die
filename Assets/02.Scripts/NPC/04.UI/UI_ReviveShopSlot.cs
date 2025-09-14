using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ReviveShopSlot : MonoBehaviour
{
    public int SlotIndex;
    public Image IconImage;

    public void Start()
    {
        IconImage.gameObject.SetActive(false);
        ReviveShopManager.Instance.OnReviveSlotUpdated[SlotIndex] += UpdateSlotUI;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MasterAudio.PlaySound("ButtonClick");
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ReviveShopManager.Instance.OnClickMouseLeft(SlotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ReviveShopManager.Instance.OnClickMouseRight(SlotIndex);
        }
    }

    public void UpdateSlotUI()
    {
        ItemInstance itemInstanceInSlot = ReviveShopManager.Instance.ReviveShopInventory.SlotList[SlotIndex].ItemInstance;
        if (itemInstanceInSlot == null)
        {
            IconImage.gameObject.SetActive(false);
            return;
        }

        IconImage.sprite = ItemManager.Instance.GetItem(itemInstanceInSlot.ID).ItemDefinition.Icon;
        IconImage.gameObject.SetActive(true);
    }
}