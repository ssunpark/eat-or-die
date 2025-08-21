using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_SeedItemButton : MonoBehaviour
{
    public Image IconImage;

    private ItemProfile _itemProfile;
    public void Setup(ItemProfile itemProfile)
    {
        _itemProfile = itemProfile;
        IconImage.sprite = itemProfile.ItemDefinition.Icon;
    }

    public void OnClick()
    {
        SeedShopPanelManager.Instance.UpdateSeedDetail(_itemProfile.ItemDefinition.ID);
        MasterAudio.PlaySound("ButtonClick");
    }
}
