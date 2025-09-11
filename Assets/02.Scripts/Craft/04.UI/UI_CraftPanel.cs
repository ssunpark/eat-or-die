using System.Linq;
using UnityEngine;

public class UI_CraftPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Craft;
    public GameObject CraftPanel;
    public UI_CraftItemList CraftItemList;
    private bool _isInitalized;
    public UI_CraftDetailPanel UICraftDetailPanel;

    

    private void Start()
    {
        CraftPanel.SetActive(false);
    }

    public override void Open()
    {
        base.Open();
        if (!_isInitalized)
        {
            Init();
            _isInitalized = true;
        }
    }

    private void Init()
    {
        CraftItemList.Init();
        UICraftDetailPanel.RefreshCraftCount();
        
        var cookingPotRecipe = CraftRecipeManager.Instance.CraftRecipeList
            .FirstOrDefault(recipe => recipe.CraftResultID == 400001);
        if (cookingPotRecipe != null)
        {
            CraftRecipeUIManager.Instance.SelectCraftItem(cookingPotRecipe);
        }
    }
}