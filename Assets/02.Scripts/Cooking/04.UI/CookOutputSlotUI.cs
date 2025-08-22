using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookOutputSlotUI : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI QuantityText;

    public Color lockedColor = Color.gray;

    public Sprite unknownIcon; 

    private void Start()
    {
        Clear();
        CookingManager.Instance.OnCookingSlotUpdated[0] += UpdateSlotUI_ActualResult;
        CookingManager.Instance.OnCookingSlotUpdated[1] += UpdateSlotUI_ActualResult;
    }
    
    private void UpdateSlotUI_ActualResult()
    {
        if (CookingManager.Instance.HasEmptySlot())
        {
            Clear();
            return;
        }

        var quantityToCook = Mathf.Min(
            CookingManager.Instance.IngredientInventory.SlotList[0].ItemInstance.Quantity,
            CookingManager.Instance.IngredientInventory.SlotList[1].ItemInstance.Quantity
        );

        var itemId = CookingManager.Instance.TryCook();
        var resultItem = ItemManager.Instance.GetItem(itemId);

        // 재료 1개 이상일 떄부터 갯수 텍스트 띄우기
        // if (quantityToCook > 1)
        // {
        QuantityText.text = quantityToCook.ToString();
        // }

        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == resultItem.ItemDefinition.ID);
        var isKnown = RecipePanelUIManager.Instance.IsKnownRecipe(recipe.ID);
        var canMake = RecipePanelUIManager.Instance.CanMakeRecipe(recipe);
        
        if (!isKnown)
        {
            IconImage.sprite = unknownIcon;
            IconImage.color = lockedColor;
            IconImage.gameObject.SetActive(true);
            IconImage.color = lockedColor;
            return;
        }

        if (resultItem != null)
        {
            if (recipe.ResultID == 200120 || recipe.ResultID == 200121 || recipe.ResultID == 200122)
            {
                IconImage.sprite = unknownIcon;
                IconImage.gameObject.SetActive(true);
                IconImage.color = lockedColor;
            }

            else
            {
                IconImage.sprite = resultItem.ItemDefinition.Icon;
                IconImage.gameObject.SetActive(true);
                IconImage.color = lockedColor;
            }
        }
        
        
    }

    private void Clear()
    {
        IconImage.color = Color.clear;
        QuantityText.text = "";
    }
}