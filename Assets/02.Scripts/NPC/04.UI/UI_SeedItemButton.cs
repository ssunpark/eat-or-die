using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_SeedItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI SeedNameText;

    private ItemProfile _itemProfile;
    public void Setup(ItemProfile itemProfile)
    {
        _itemProfile = itemProfile;
        IconImage.sprite = itemProfile.ItemDefinition.Icon;
        // SeedNameText.text = itemInfo.ItemData.Name;
    }

    public void OnClick()
    {
        SeedShopPanelManager.Instance.UpdateSeedDetail(_itemProfile.ItemDefinition.ID);
        MasterAudio.PlaySound("ButtonClick");
    }
}
