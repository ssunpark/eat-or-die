using System.Collections.Generic;
using UnityEngine;

public class UI_SeedItemList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private Dictionary<int, UI_SeedItemButton> _buttonDict = new Dictionary<int, UI_SeedItemButton>();
    private bool _isInitialized = false;

    private void Awake()
    {
        // 무조건 구독 (SeedShopPanelManager가 더 먼저 Awake/Start 돼도 상관없음)
        SeedShopPanelManager.Instance.OnSeedListUpdated += Init;
    }

    private void OnEnable()
    {
        RefreshButtons(); // 항상 버튼 상태 갱신
    }

    private void OnDisable()
    {
        if (SeedShopPanelManager.Instance != null)
            SeedShopPanelManager.Instance.OnSeedListUpdated -= Init;
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
            // button.Setup(itemInfo);
            obj.SetActive(false);

            _buttonDict[itemInfo.ItemData.ID] = button;
        }

        RefreshButtons(); // Init 끝나면 바로 갱신
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