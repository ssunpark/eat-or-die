using UnityEngine;

public class UI_SeedShopPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Shop;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            InputReader.Instance.ReleaseControl();
        }
    }
}