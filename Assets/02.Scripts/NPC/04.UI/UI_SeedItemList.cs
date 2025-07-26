using System.Collections.Generic;
using UnityEngine;

public class UI_SeedItemList : MonoBehaviour
{
        public GameObject Container;
    public GameObject ButtonPrefab;

    private Dictionary<int, UI_SeedItemButton> _buttonDict = new Dictionary<int, UI_SeedItemButton>();
    private bool _isInitialized = false;
    private bool _isSubscribed = false;

    private void OnEnable()
    {
        // SeedItems가 이미 있으면 Init 바로 실행
        if (!_isInitialized && SeedShopPanelManager.Instance.SeedItems.Length > 0)
        {
            Init();
        }
        // 아니면 이벤트 대기
        else if (!_isSubscribed)
        {
            SeedShopPanelManager.Instance.OnSeedListUpdated += Init;
            _isSubscribed = true;
        }

        RefreshButtons();
    }

    private void OnDisable()
    {
        if (_isSubscribed)
        {
            SeedShopPanelManager.Instance.OnSeedListUpdated -= Init;
            _isSubscribed = false;
        }
    }

    public void Init()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        AItemInfo[] seedItems = SeedShopPanelManager.Instance.SeedItems;
        if (seedItems == null || seedItems.Length == 0)
        {
            Debug.LogWarning("[SeedItemList] SeedItems가 비어 있습니다.");
            return;
        }

        _buttonDict.Clear();

        foreach (AItemInfo itemInfo in seedItems)
        {
            GameObject obj = Instantiate(ButtonPrefab, Container.transform);
            UI_SeedItemButton button = obj.GetComponent<UI_SeedItemButton>();
            button.Setup(itemInfo);
            obj.SetActive(false);

            _buttonDict[itemInfo.ItemData.ID] = button;
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

        AItemInfo[] seedItems = SeedShopPanelManager.Instance.SeedItems;
        foreach (var item in seedItems)
        {
            if (_buttonDict.TryGetValue(item.ItemData.ID, out var button))
            {
                button.gameObject.SetActive(true);
            }
        }
    }
}