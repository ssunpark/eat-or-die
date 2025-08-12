using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftItemButton : MonoBehaviour
{
    public Image IconImage;
    public TextMeshProUGUI ItemNameText;
    public Image CanCraftIcon;

    private CraftRecipe _data;
    public int CraftRecipeID => _data.CraftResultID;
    private ItemProfile _itemProfile;

    public void Init(CraftRecipe data)
    {
        _data = data;
        ItemNameText.text = _data.CraftRecipeName;
        ItemProfile itemProfile = ItemManager.Instance.GetItem(_data.CraftResultID);
        IconImage.sprite = itemProfile.ItemDefinition.Icon;
    }

    public void Refresh(CraftRecipe data)
    {
        Debug.Log("인벤토리에서 만들 수 있는지 아이콘으로 표시하는 부분은 리프레시해야 함!");
    }

    // 이게 사실상 리프레시할때마다 호출 필요한 함수
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

    public void OnClickItemButton()
    {
        Debug.Log("버튼 클릭!!!");
        Debug.Log("이 부분은 이제 각각의 아이템에 대한 세부 사항을 리프레시하는 기능 들어감");
    }

    // 이건 나중에 setdetail 창에서 제작하기 클릭할때 연결할 부분
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
