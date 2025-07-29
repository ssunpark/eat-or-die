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
}
