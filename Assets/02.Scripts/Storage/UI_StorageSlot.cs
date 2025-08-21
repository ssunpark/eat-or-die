using DarkTonic.MasterAudio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StorageSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int SlotIndex;
    public Image IconImage;
    public GameObject QuantityIndicator;
    public TextMeshProUGUI QuantityText;
    public TextMeshProUGUI LabelText;
    public UI_Tooltip ItemTooltip;
    public CanvasGroup Selected;
    
    private bool _isSlotEmpty => !IconImage.gameObject.activeInHierarchy;
    
    public void Initialize(int slotIndex)
    {
        SlotIndex = slotIndex;
        IconImage.gameObject.SetActive(false);
        QuantityIndicator.gameObject.SetActive(false);
        if (ItemTooltip == null)
        {
            ItemTooltip = GetComponent<UI_Tooltip>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MasterAudio.PlaySound("ButtonClick");
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
            QuantityIndicator.gameObject.SetActive(false);
            LabelText.text = string.Empty;
            return;
        }
        
        ItemProfile itemProfile = ItemManager.Instance.GetItem(itemInstanceInStorage.ID);
        
        IconImage.sprite = itemProfile.ItemDefinition.Icon;
        QuantityText.text = itemInstanceInStorage.Quantity.ToString();
        LabelText.text = itemProfile.ItemDefinition.Name;
        IconImage.gameObject.SetActive(true);
        QuantityIndicator.gameObject.SetActive(itemInstanceInStorage.Quantity > 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Selected?.DOFade(1.0f, 0.1f).SetEase(Ease.InOutQuad);
        if (_isSlotEmpty) return;
		
        ItemTooltip.OnPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Selected?.DOFade(0.0f, 0.1f).SetEase(Ease.InOutQuad);
        ItemTooltip.OnPointerExit();
    }
}