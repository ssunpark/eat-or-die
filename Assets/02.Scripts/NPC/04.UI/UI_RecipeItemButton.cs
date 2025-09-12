using UnityEngine;

public class UI_RecipeItemButton : MonoBehaviour
{
    private ItemProfile _itemProfile;

    public void Setup(ItemProfile itemProfile)
    {
        _itemProfile = itemProfile;
    }

    public void OnClick()
    {
        RecipeShopManager.Instance.UpdateRecipeDetail(_itemProfile.ItemDefinition.ID);
    }
}
