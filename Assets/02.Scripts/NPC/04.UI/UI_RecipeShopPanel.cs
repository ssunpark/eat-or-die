using System;
using UnityEngine;

public class UI_RecipeShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public event Action OnClose;

    public override void Close()
    {
        base.Close();
        OnClose?.Invoke();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}
