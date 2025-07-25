using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_SeedItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI SeedNameText;

    // private AItemInfo _itemInfo;

    public void Setup(ItemData itemData)
    {
        // _itemInfo = itemInfo;
        IconImage.sprite = itemData.Icon;
        SeedNameText.text = itemData.Name;
    }

    public void OnClick()
    {
        // SeedShopPanelManager.Instance.UpdateSeedDetail(item.ID);
    }
}
