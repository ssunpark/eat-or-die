using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI ItemNameText;
    
    private CraftRecipe _craftRecipe;
    private ItemProfile _itemProfile;
    
    public void Refresh(CraftRecipe craftRecipe, ItemProfile itemProfile)
    {
        _craftRecipe = craftRecipe;
        _itemProfile = itemProfile;

        IconImage.sprite = itemProfile.ItemDefinition.Icon;
        ItemNameText.text = craftRecipe.CraftRecipeName;
    }

    public void CanInteractable()
    {
        int haveMat1 = InventoryManager.Instance.GetItemCount(_craftRecipe.CraftMaterial1ID);
        int haveMat2 = InventoryManager.Instance.GetItemCount(_craftRecipe.CraftMaterial2ID);

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
        bool consumedMat1 = InventoryManager.Instance.TryConsumeItem(_craftRecipe.CraftMaterial1ID, _craftRecipe.CraftMaterial1Count);
        bool consumedMat2 = InventoryManager.Instance.TryConsumeItem(_craftRecipe.CraftMaterial2ID, _craftRecipe.CraftMaterial2Count);

        if (!consumedMat1 || !consumedMat2)
        {
            Debug.Log("재료가 부족하여 제작에 실패했습니다.");
            return;
        }

        ItemInstance craftedItemInstance = new ItemInstance(_itemProfile, 1);
        InventoryManager.Instance.AddItemToInventory(craftedItemInstance);

        Debug.Log($"{_itemProfile.ItemDefinition.Name} 제작 성공!");
        
    }
}
