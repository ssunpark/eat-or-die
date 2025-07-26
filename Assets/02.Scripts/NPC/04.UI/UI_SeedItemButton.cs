using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_SeedItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI SeedNameText;

    private AItemInfo _itemInfo;
    public void Setup(AItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        IconImage.sprite = itemInfo.ItemData.Icon;
        SeedNameText.text = itemInfo.ItemData.Name;
    }

    public void OnClick()
    {
        SeedShopPanelManager.Instance.UpdateSeedDetail(_itemInfo.ItemData.ID);
    }
}
