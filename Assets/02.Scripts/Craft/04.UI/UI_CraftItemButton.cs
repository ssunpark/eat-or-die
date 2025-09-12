using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;

    public Image CraftIndicatiorIcon;
    public GameObject ItemSelectedOutline;

    private CraftRecipe _data;
    public int CraftRecipeID => _data.CraftResultID;

    public void Init(CraftRecipe data)
    {
        _data = data;
        ItemProfile itemProfile = ItemManager.Instance.GetItem(_data.CraftResultID);
        IconImage.sprite = itemProfile.ItemDefinition.Icon;
        SetSelected(false);
    }
    
    public void CanCraft()
    {
        var haveMat1 = UnifiedInventoryManager.Instance.GetItemCount(_data.CraftMaterial1ID);
        var haveMat2 = UnifiedInventoryManager.Instance.GetItemCount(_data.CraftMaterial2ID);

        var canCraft = haveMat1 >= _data.CraftMaterial1Count &&
                       haveMat2 >= _data.CraftMaterial2Count;

        if (canCraft)
        {
            CraftIndicatiorIcon.gameObject.SetActive(true);
        }
        else
        {
            CraftIndicatiorIcon.gameObject.SetActive(false);
        }
    }

    public void OnClickItemButton()
    {
        CraftRecipeUIManager.Instance.SelectCraftItem(_data);
        MasterAudio.PlaySound("ButtonClick");
    }

    public void SetSelected(bool isSelected)
    {
        if (ItemSelectedOutline != null)
        {
            ItemSelectedOutline.SetActive(isSelected);
        }
    }
}
