using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookInputSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;
    private Recipe _data;

    public void Start()
    {
        IconImage.gameObject.SetActive(false);
        QuantityText.gameObject.SetActive(false);
        CookingManager.Instance.OnCookingSlotUpdated[SlotIndex] += UpdateSlotUI;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        { 
            CookingManager.Instance.OnClickMouseLeft(SlotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            CookingManager.Instance.OnClickMouseRight(SlotIndex);
        }
    }
    
    public void UpdateSlotUI()
    {
        ItemInstance itemInstanceInSlot = CookingManager.Instance.IngredientInventory.SlotList[SlotIndex].ItemInstance;
        if (itemInstanceInSlot == null)
        {
            IconImage.gameObject.SetActive(false);
            QuantityText.gameObject.SetActive(false);
            return;
        }
        
        IconImage.sprite = ItemManager.Instance.GetItem(itemInstanceInSlot.ID).ItemDefinition.Icon;
        QuantityText.text = itemInstanceInSlot.Quantity.ToString();
        IconImage.gameObject.SetActive(true);
        QuantityText.gameObject.SetActive(itemInstanceInSlot.Quantity > 0); // 개수 1개 이상일 때부터 갯수 텍스트 띄우기
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        
        if (_data == null || !RecipePanelUIManager.Instance.IsKnownRecipe(_data.ID))
        {
            return;
        }

        var itemProfile = ItemManager.Instance.GetItem(_data.ResultID);
        if (itemProfile == null)
        {
            return;
        }

        var sb = new StringBuilder();

        sb.Append($"<color=#7BD9B2><b>{itemProfile.ItemDefinition.Name}</b></color>\n\n");
        sb.Append($"{itemProfile.ItemDefinition.Description}\n\n"); // 설명

        var extraDescription = string.Join("  ", itemProfile.ItemDefinition.ExtraDescription);
        extraDescription = RichTextUtil.RecolorAll(extraDescription, "#E44962");
        sb.Append($"{extraDescription}");

        TooltipManager.Instance.Show(sb.ToString());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}
