using UnityEngine;

public class UI_SeedShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    public override void Open()
    {
        base.Open();
        InitUI();
    }

    private void InitUI()
    {
        // 아이템 디테일 ui 초기화
        // 소지한 골드량 초기화
    }
}