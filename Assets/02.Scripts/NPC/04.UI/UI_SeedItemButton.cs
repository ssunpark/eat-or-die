using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

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
        SeedShopManager.Instance.UpdateSeedDetail(_itemProfile.ItemDefinition.ID);
        MasterAudio.PlaySound("ButtonClick");
    }
}
