using System;
using UnityEngine;

public class UI_RecipeShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public GameObject BlockPopup;
    public event Action OnClose;
    private bool _isBlocked = false;
                                                                                                                                                                                                   
    private void Start()
    {
        gameObject.SetActive(false);
        UI_RecipeItemDetail.OnItemPurchased += OnItemPurchased;
        RecipeShopEvents.OnRecipeScrollUsed += OnRecipeUnlocked;
    }

    public override void Open()
    {                              
        base.Open();
        UpdateBlockVisibility();
    }
    public override void Close()                             
    {                   
        base.Close();
        OnClose?.Invoke();
    }
    
    private void OnItemPurchased()
    {
        _isBlocked = true;
        UpdateBlockVisibility();
    }

    private void OnRecipeUnlocked(int recipeID)
    {
        _isBlocked = false;
        UpdateBlockVisibility();
    }
    
    private void UpdateBlockVisibility()
    {
        if (BlockPopup != null)
        {
            BlockPopup.SetActive(_isBlocked);
        }
    }
}
