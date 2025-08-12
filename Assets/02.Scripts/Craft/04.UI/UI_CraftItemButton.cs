using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CraftCategory
{
    All, // 전체
    Tool, // 도구
    Weapon, // 무기
    Equipment // 장비
}

public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI ItemNameText;
    public Image CanCraftIcon;

    private CraftRecipe _data;
    public int CraftRecipeID => _data.CraftResultID;
    private ItemProfile _itemProfile;

    public void Refresh(CraftRecipe data)
    {
        _data = data;
        // 인벤토리 기준으로 만들 수 있는지에 대한 여부 리프레시 > 아이콘이 인벤토리 상태에 따라 만들 수 있으면 활성화, 만들 수 없으면 비활성화
        // IconImage.sprite = itemProfile.ItemDefinition.Icon;
        // ItemNameText.text = data.CraftRecipeName;
    }

    public void CanCraft()
    {
        var haveMat1 = InventoryManager.Instance.GetItemCount(_data.CraftMaterial1ID);
        var haveMat2 = InventoryManager.Instance.GetItemCount(_data.CraftMaterial2ID);

        var canCraft = haveMat1 >= _data.CraftMaterial1Count &&
                       haveMat2 >= _data.CraftMaterial2Count;
        
        Button button = GetComponent<Button>();
        button.interactable = canCraft;
        
        ColorBlock colors = button.colors;
        colors.normalColor = canCraft ? Color.white : Color.gray;
        button.colors = colors;
    }

    public void OnClick()
    {
        var consumedMat1 = InventoryManager.Instance.TryConsumeItem(_data.CraftMaterial1ID, _data.CraftMaterial1Count);
        var consumedMat2 = InventoryManager.Instance.TryConsumeItem(_data.CraftMaterial2ID, _data.CraftMaterial2Count);

        if (!consumedMat1 || !consumedMat2)
        {
            Debug.Log("재료가 부족하여 제작에 실패했습니다.");
            return;
        }

        ItemInstance craftedItemInstance = new ItemInstance(_itemProfile, 1);
        InventoryManager.Instance.AddItemToInventory(craftedItemInstance);

        Debug.Log($"{_itemProfile.ItemDefinition.Name} 제작 성공!");
        
    }
}
