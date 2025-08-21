using UnityEngine;

public class UI_SeedShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    private void Start()
    {
        gameObject.SetActive(false);
    }
}