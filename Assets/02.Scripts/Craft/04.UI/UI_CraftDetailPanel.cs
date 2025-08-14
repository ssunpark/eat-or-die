using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftDetailPanel : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _durabilityText; // 내구도
    [SerializeField] private TextMeshProUGUI _attributeDescriptionText;
    [SerializeField] private Color _richTextColor;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _currentCraftReipeCountText;

    [SerializeField] private GameObject _ingredientItemContainer;
    [SerializeField] private GameObject _ingredientItemPrefab;

    private CraftRecipe _currentCraftRecipe;
    private ItemProfile _itemProfile;
    private readonly List<UI_CraftIgredientButton> _ingredientButtons = new();

    private void OnEnable()
    {
        UnifiedInventoryManager.Instance.OnPossessionUpdated += RefreshCraftCount;
    }

    private void OnDisable()
    {
        if (UnifiedInventoryManager.Instance != null)
        {
            UnifiedInventoryManager.Instance.OnPossessionUpdated += RefreshCraftCount;
        }
    }

    public void UpdateDetails(CraftRecipe craftRecipe)
    {
        if (craftRecipe == null)
        {
            return;
        }

        _currentCraftRecipe = craftRecipe;

        _itemProfile = ItemManager.Instance.GetItem(_currentCraftRecipe.CraftResultID);
        
        var itemExtraDescriptionFactory = new ItemExtraDescriptionFactory();
        _itemIcon.sprite = _itemProfile.ItemDefinition.Icon;
        _nameText.text = _itemProfile.ItemDefinition.Name;

        // 내구도가 무한인 경우에 대하여 예외 처리
        var durabilityText = _itemProfile.ItemDefinition.Type == EItemType.Craft
            ? "무한"
            : _itemProfile.ItemDefinition.MaxDurability.ToString("n0");
        _durabilityText.text = $"내구도  <color=#7BD9B2>{durabilityText}</color>";

        var extraDescriotion = string.Join("  ", _itemProfile.ItemDefinition.ExtraDescription);
        extraDescriotion = RichTextUtil.RecolorAll(extraDescriotion, "#E44962");
        _attributeDescriptionText.text = extraDescriotion;

        _descriptionText.text = _itemProfile.ItemDefinition.Description;

        var resultItemCount = UnifiedInventoryManager.Instance.GetItemCount(craftRecipe.CraftResultID);
        _currentCraftReipeCountText.text = $"{resultItemCount}";
    }

    public void CreateIngredientButtons()
    {
        for (var i = 0; i < 2; i++) // 재료 2개만 있으니까 일단 이렇게 만들고, 나중에 수정
        {
            var buttonObj = Instantiate(_ingredientItemPrefab, _ingredientItemContainer.transform);
            var craftRecipeButton = buttonObj.GetComponent<UI_CraftIgredientButton>();
            _ingredientButtons.Add(craftRecipeButton);
        }

        RefreshCraftCount();
    }

    public void RefreshCraftCount()
    {
        if (_currentCraftRecipe == null)
        {
            return;
        }

        _ingredientButtons[0].Refresh(_currentCraftRecipe.CraftMaterial1ID, _currentCraftRecipe.CraftMaterial1Count);
        _ingredientButtons[1].Refresh(_currentCraftRecipe.CraftMaterial2ID, _currentCraftRecipe.CraftMaterial2Count);
    }

    // 이건 나중에 setdetail 창에서 제작하기 클릭할때 연결할 부분
    public void OnClickPurchaseButton()
    {
        if (_currentCraftRecipe == null || _itemProfile == null) return;

        // 4. 재료를 소모하기 전에, 만들 수 있는지 '확인'부터 합니다.
        var haveMat1 = UnifiedInventoryManager.Instance.GetItemCount(_currentCraftRecipe.CraftMaterial1ID);
        var haveMat2 = UnifiedInventoryManager.Instance.GetItemCount(_currentCraftRecipe.CraftMaterial2ID);
        
        bool canCraft = haveMat1 >= _currentCraftRecipe.CraftMaterial1Count;
        if (_currentCraftRecipe.CraftMaterial2ID > 0)
        {
            canCraft &= haveMat2 >= _currentCraftRecipe.CraftMaterial2Count;
        }

        if (!canCraft)
        {
            Debug.Log("재료가 부족하여 제작에 실패했습니다.");
            return;
        }

        // 5. 만들 수 있는게 확인되면, 재료를 '소모'합니다.
        UnifiedInventoryManager.Instance.TryConsumeLocalItem(_currentCraftRecipe.CraftMaterial1ID, _currentCraftRecipe.CraftMaterial1Count);
        if (_currentCraftRecipe.CraftMaterial2ID > 0)
        {
            UnifiedInventoryManager.Instance.TryConsumeLocalItem(_currentCraftRecipe.CraftMaterial2ID, _currentCraftRecipe.CraftMaterial2Count);
        }

        // 6. 아이템을 생성하고 인벤토리에 추가합니다.
        var craftedItemInstance = new ItemInstance(_itemProfile, 1);
        UnifiedInventoryManager.Instance.AddItem(craftedItemInstance);

        Debug.Log($"{_itemProfile.ItemDefinition.Name} 제작 성공!");
    }
}