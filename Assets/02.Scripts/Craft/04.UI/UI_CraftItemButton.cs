using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI ItemNameText;
    
    private CraftRecipe _craftRecipe;
    private AItemInfo _itemInfo;
    
    public void Refresh(CraftRecipe craftRecipe, AItemInfo itemInfo)
    {
        _craftRecipe = craftRecipe;
        _itemInfo = itemInfo;

        IconImage.sprite = itemInfo.ItemData.Icon;
        ItemNameText.text = craftRecipe.CraftRecipeName;
    }

    public void CanInteractable()
    {
        int haveMat1 = InventoryManager.Instance.Inventory.GetItemCount(_craftRecipe.CraftMaterial1ID);
        int haveMat2 = InventoryManager.Instance.Inventory.GetItemCount(_craftRecipe.CraftMaterial2ID);

        bool canCraft = haveMat1 >= _craftRecipe.CraftMaterial1Count &&
                        haveMat2 >= _craftRecipe.CraftMaterial2Count;
        
        Button button = GetComponent<Button>();
        button.interactable = canCraft;
        
        ColorBlock colors = button.colors;
        colors.normalColor = canCraft ? Color.white : Color.gray;
        button.colors = colors;
    }

    public void OnClick()
    {
        bool consumedMat1 = InventoryManager.Instance.Inventory.TryConsumeItem(_craftRecipe.CraftMaterial1ID, _craftRecipe.CraftMaterial1Count);
        bool consumedMat2 = InventoryManager.Instance.Inventory.TryConsumeItem(_craftRecipe.CraftMaterial2ID, _craftRecipe.CraftMaterial2Count);

        if (!consumedMat1 || !consumedMat2)
        {
            Debug.LogWarning("재료가 부족하여 제작에 실패했습니다.");
            return;
        }

        Item craftedItem = new Item(_itemInfo, 1);
        InventoryManager.Instance.PickItemFromGround(craftedItem);

        Debug.Log($"{_itemInfo.ItemData.Name} 제작 성공!");
        
    }
}
