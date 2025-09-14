using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_RecipeItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private ItemProfile[] _recipeItems;
    private List<UI_RecipeItemButton> _buttons = new List<UI_RecipeItemButton>();
    private bool _isSubscribed = false;

    private void OnEnable()
    {
        SubscribeToEvents();
        RefreshButtons();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (RecipeShopManager.Instance != null && !_isSubscribed)
        {
            RecipeShopManager.Instance.OnRecipeListUpdated += OnRecipeListUpdated;
            _isSubscribed = true;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (RecipeShopManager.Instance != null && _isSubscribed)
        {
            RecipeShopManager.Instance.OnRecipeListUpdated -= OnRecipeListUpdated;
            _isSubscribed = false;
        }
    }

    private void OnRecipeListUpdated()
    {
        // 레시피 목록이 업데이트되면 즉시 새로고침
        RefreshButtonsInternal();
        Debug.Log("[RecipeItemList] 레시피 목록 업데이트로 인한 즉시 새로고침");
    }

    public void RefreshButtons()
    {
        RefreshButtonsInternal();
    }

    private void RefreshButtonsInternal()
    {
        if (RecipeShopManager.Instance == null)
        {
            Debug.Log("[RecipeItemList] RecipeShopManager.Instance가 null입니다.");
            return;
        }

        _recipeItems = RecipeShopManager.Instance.RecipeItems;

        if (_recipeItems == null || _recipeItems.Length == 0)
        {
            Debug.Log("[RecipeItemList] RecipeItems가 비어 있습니다. UpdateRecipeShopList가 먼저 호출되어야 합니다.");
            ClearAllButtons();
            return;
        }

        // 기존 버튼들을 모두 삭제
        ClearAllButtons();

        // 새로운 미해금 레시피 버튼들을 생성 (최대 8개)
        for (int i = 0; i < _recipeItems.Length; i++)
        {
            GameObject obj = Instantiate(ButtonPrefab, Container.transform);
            UI_RecipeItemButton button = obj.GetComponent<UI_RecipeItemButton>();
            button.Setup(_recipeItems[i]);
            obj.SetActive(true);

            _buttons.Add(button);
        }

        Debug.Log($"[RecipeItemList] {_buttons.Count}개의 새로운 레시피 버튼이 생성되었습니다.");
    }

    private void ClearAllButtons()
    {
        foreach (var button in _buttons)
        {
            if (button != null && button.gameObject != null)
            {
                DestroyImmediate(button.gameObject);
            }
        }
        _buttons.Clear();
    }
}
