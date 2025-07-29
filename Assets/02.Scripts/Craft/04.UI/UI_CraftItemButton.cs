using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 수현
public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI ItemNameText;
    
    private CraftRecipe _craftRecipe;
    private AItemInfo _itemInfo;
    
    public void Refresh(CraftRecipe craftRecipe, AItemInfo itemInfo)
    {
        _craftRecipe = craftRecipe;
        _itemInfo = itemInfo;

        IconImage.sprite = itemInfo.ItemData.Icon;
        ItemNameText.text = craftRecipe.CraftRecipeName;
    }

    public void CanInteractable()
    {
        // 인벤토리에 있는 아이템 개수에 따라서 활성화/회색처리 로직 부르기
    }
}
