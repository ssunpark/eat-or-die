using System;
using UnityEngine;

public class UI_SeedShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public event Action OnClose;

    public override void Open()
    {
        base.Open();
        InitUI();
    }

    public override void Close()
    {
        base.Close();
        OnClose?.Invoke();
    }

    private void InitUI()
    {
        // 아이템 디테일 ui 초기화
        // 소지한 골드량 초기화
    }
}