using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;
    private ItemProfile[] _recipeItems;

    private Dictionary<int, UI_RecipeItemButton> _buttonDict = new Dictionary<int, UI_RecipeItemButton>();
    private bool _isInitialized = false;
    private bool _isSubscribed = false;

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            Init();
        }

        RefreshButtons();
    }

    private void OnDisable()
    {
        if (RecipeShopManager.Instance == null) return;
        if (_isSubscribed)
        {
            RecipeShopManager.Instance.OnRecipeListUpdated -= Init;
            _isSubscribed = false;
        }
    }

    public void Init()
    {
        _recipeItems = RecipeShopManager.Instance.RecipeItems;
        if (_recipeItems == null || _recipeItems.Length == 0)
        {
            if (!_isSubscribed)
            {
                RecipeShopManager.Instance.OnRecipeListUpdated += Init;
                _isSubscribed = true;
            }
            
            Debug.Log("[RecipeItemList] RecipeItems가 비어 있습니다.");
            return;
        }

        if (_isSubscribed)
        {
            RecipeShopManager.Instance.OnRecipeListUpdated -= Init;
            _isSubscribed = false;
        }

        _isInitialized = true;
        _buttonDict.Clear();

        foreach (ItemProfile itemInfo in _recipeItems)
        {
            GameObject obj = Instantiate(ButtonPrefab, Container.transform);
            UI_RecipeItemButton button = obj.GetComponent<UI_RecipeItemButton>();
            button.Setup(itemInfo);
            obj.SetActive(false);

            _buttonDict[itemInfo.ItemDefinition.ID] = button;
        }

        RefreshButtons();
    }

    private void RefreshButtons()
    {
        if (!_isInitialized) return;

        foreach (var button in _buttonDict.Values)
        {
            button.gameObject.SetActive(false);
        }

        ItemProfile[] recipeItems = RecipeShopManager.Instance.RecipeItems;
        if (recipeItems == null) return;

        foreach (var item in recipeItems)
        {
            if (_buttonDict.TryGetValue(item.ItemDefinition.ID, out var button))
            {
                button.gameObject.SetActive(true);
            }
        }
    }
}
