using System;
using UnityEngine;

public class UI_RecipeShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public GameObject BlockPopup;
    public event Action OnClose;

    public override void Open()
    {
        base.Open();
        BlockPopup.SetActive(false);
        
    }

    public override void Close()
    {
        base.Close();
        OnClose?.Invoke();
        BlockPopup.SetActive(false);
    }

    private void Start()
    {
        gameObject.SetActive(false);
        UI_RecipeItemDetail.OnItemPurchased += OnItemPurchased;
        RecipeShopEvents.OnRecipeScrollUsed += OnRecipeUnlocked;
    }

    private void OnItemPurchased()
    {
        if (BlockPopup != null)
        {
            BlockPopup.SetActive(true);
            Debug.Log("[RecipeShopPanel] 구매로 인한 블록 활성화");
        }
    }

    private void OnRecipeUnlocked(int recipeID)
    {
        Debug.Log($"[RecipeShopPanel] 레시피 해금 완료로 인한 블록 해제: {recipeID}");
        if (BlockPopup != null)
        {
            BlockPopup.SetActive(false);
        }
    }
}
