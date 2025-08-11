using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookOutputSlotUI : MonoBehaviour
{
    public int SlotIndex;
    public Image IconImage;
    public TextMeshProUGUI QuantityText;

    public Color lockedColor = Color.gray;

    public Sprite unknownIcon; 

    private void Start()
    {
        // IconImage.gameObject.SetActive(false);
        Clear();
        // QuantityText.gameObject.SetActive(false);

        // CookingManager.OnCookOutputUpdated += UpdateSlotUI;
        CookingManager.Instance.OnCookingSlotUpdated[0] += UpdateSlotUI_ActualResult;
        CookingManager.Instance.OnCookingSlotUpdated[1] += UpdateSlotUI_ActualResult;
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if (eventData.button == PointerEventData.InputButton.Left)
    //     {
    //         TakeOutItem();
    //     }
    // }

    // 결과 슬롯의 아이콘과 수량 갱신
    // private void TakeOutItem()
    // {
    //     var foodInventory = CookingManager.Instance.FoodInventory;
    //     var slot = foodInventory.SlotList[SlotIndex];
    //     if (slot.IsEmpty) return;
    //
    //     if (HandEntity.Instance.IsHandEmpty)
    //     {
    //         // 한 개만 손으로 집기
    //         HandEntity.Instance.PickUpItem(foodInventory.PopSingleItemInSlot(SlotIndex));
    //     }
    //     else
    //     {
    //         if (HandEntity.Instance.ItemInstance.ID == slot.ItemInstance.ID)
    //         {
    //             // 스택 합치기
    //             ItemInstance popped = foodInventory.PopSingleItemInSlot(SlotIndex);
    //             if (!HandEntity.Instance.TryAddItem(popped))
    //             {
    //                 // 손에 더 못 넣으면 다시 인벤토리에 넣기
    //                 slot.AddItem(popped);
    //             }
    //         }
    //     }
    //
    //     UpdateSlotUI();
    // }

    // public void UpdateSlotUI()
    // {
    //     var itemInSlot = CookingManager.Instance.FoodInventory.SlotList[SlotIndex].ItemInstance;
    //     if (itemInSlot == null)
    //     {
    //         IconImage.gameObject.SetActive(false);
    //         QuantityText.gameObject.SetActive(false);
    //         return;
    //     }
    //
    //     IconImage.sprite = ItemManager.Instance.GetItem(itemInSlot.ID).ItemDefinition.Icon;
    //     QuantityText.text = itemInSlot.Quantity.ToString();
    //     IconImage.gameObject.SetActive(true);
    //     QuantityText.gameObject.SetActive(itemInSlot.Quantity > 1);
    // }

    /// <summary>
    ///     (예상 결과) 재료가 변경될 때 호출됩니다.
    /// </summary>
    /// <summary>
    ///     (미리보기용) 재료가 변경될 때 호출됩니다.
    /// </summary>
    /// <summary>
    ///     (실제 결과물용) 요리가 끝나거나 아이템을 가져갈 때 호출됩니다.
    /// </summary>
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
        // IconImage.color = Color.white;
        // IconImage.sprite = resultItem.ItemDefinition.Icon;

        // if (quantityToCook > 1)
        // {
        QuantityText.text = quantityToCook.ToString();
        // }

        var recipe = RecipeManager.Instance.RecipeList.Find(r => r.ResultID == resultItem.ItemDefinition.ID);
        var isKnown = RecipePanelUIManager.Instance.IsKnownRecipe(recipe.ID);
        var canMake = RecipePanelUIManager.Instance.CanMakeRecipe(recipe);

        // ItemProfile itemProfile = ItemManager.Instance.GetItem(recipe.ResultID);

        if (!isKnown) // 방 기준으로 습득되지 않은 레시피에 대해서
        {
            IconImage.sprite = unknownIcon;
            IconImage.color = lockedColor;
            IconImage.gameObject.SetActive(true);
            IconImage.color = lockedColor;
            return;
        }

        if (resultItem != null)
        {
            IconImage.sprite = resultItem.ItemDefinition.Icon;
            IconImage.gameObject.SetActive(true);

            IconImage.color = lockedColor;
        }
        
        
    }

    private void Clear()
    {
        IconImage.color = Color.clear;
        QuantityText.text = ""; //
    }
}