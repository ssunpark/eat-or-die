using System;
using UnityEngine;

public class UI_ReviveShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public event Action OnClose;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public override void Close()
    {
        base.Close();
        OnClose?.Invoke();
    }

}