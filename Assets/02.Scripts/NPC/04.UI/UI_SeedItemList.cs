using System.Collections.Generic;
using UnityEngine;

public class UI_SeedItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;
    private ItemProfile[] _seedItems;

    private Dictionary<int, UI_SeedItemButton> _buttonDict = new Dictionary<int, UI_SeedItemButton>();
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
        if (SeedShopPanelManager.Instance == null) return;
        if (_isSubscribed)
        {
            SeedShopPanelManager.Instance.OnSeedListUpdated -= Init;
            _isSubscribed = false;
        }
    }

    public void Init()
    {
        _seedItems = SeedShopPanelManager.Instance.SeedItems;
        if (_seedItems == null || _seedItems.Length == 0)
        {
            if (!_isSubscribed)
            {
                SeedShopPanelManager.Instance.OnSeedListUpdated += Init;
                _isSubscribed = true;
            }
            
            Debug.Log("[SeedItemList] SeedItems가 비어 있습니다.");
            return;
        }

        if (_isSubscribed)
        {
            SeedShopPanelManager.Instance.OnSeedListUpdated -= Init;
            _isSubscribed = false;
        }

        _isInitialized = true;
        _buttonDict.Clear();

        foreach (ItemProfile itemInfo in _seedItems)
        {
            GameObject obj = Instantiate(ButtonPrefab, Container.transform);
            UI_SeedItemButton button = obj.GetComponent<UI_SeedItemButton>();
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

        ItemProfile[] seedItems = SeedShopPanelManager.Instance.SeedItems;
        if (seedItems == null) return;

        foreach (var item in seedItems)
        {
            if (_buttonDict.TryGetValue(item.ItemDefinition.ID, out var button))
            {
                button.gameObject.SetActive(true);
            }
        }
    }
}