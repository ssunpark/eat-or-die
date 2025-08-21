using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 아이템 세부사항의 동적 데이터 UI
public class UI_ItemPurchase : MonoBehaviour
{
    [Header("UI 연결")]
    public Slider QuantitySlider;
    public Button PlusButton;
    public Button MinusButton;
    public Button BuyButton;
    public TextMeshProUGUI QuantityText;
    public TextMeshProUGUI TotalPriceText;
    public TextMeshProUGUI CurrentGoldText;
    
    private ItemProfile _itemProfile;
    private NpcItem _npcItem;
    private int _selectedCount;
    private int _maxCount;
    private int _unitPrice;
    private bool _isUpdatingSlider = false;

    private int TotalPrice => _unitPrice * _selectedCount;
    
    private void Start()
    {
        QuantitySlider.minValue = 1;
        QuantitySlider.maxValue = 99;
        QuantitySlider.wholeNumbers = true;
        QuantitySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnEnable()
    {
        CurrencyManager.Instance.OnCurrencyChanged += OnGoldAmountChanged;
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= OnGoldAmountChanged;
        }
    }

    public void Init(ItemProfile itemProfile, NpcItem npcItem, int maxCount)
    {
        _itemProfile = itemProfile;
        _npcItem = npcItem;
        _unitPrice = npcItem.Price;
        _maxCount = Mathf.Clamp(maxCount, 1, 99);

        SelectCount(1);
        UpdateCurrentGoldText(CurrencyManager.Instance.Get(ECurrencyType.Gold));
    }
    
    private void UpdateCurrentGoldText(int currentGold)
    {
        CurrentGoldText.text = currentGold.ToString();
    }

    private void OnGoldAmountChanged(ECurrencyType currency, int currentAmount)
    {
        if (currency != ECurrencyType.Gold) return;

        int newMax = GetMaxBuyableCount(currentAmount);
        UpdateMaxCount(newMax);
        UpdateCurrentGoldText(currentAmount);
    }

    private int GetMaxBuyableCount(int currentGold)
    {
        int stockLimit = _npcItem.IsInfinite ? 99 : _npcItem.StockQuantity;
        int goldLimit = currentGold / _npcItem.Price;
        return Mathf.Clamp(Mathf.Min(stockLimit, goldLimit), 1, 99);
    }

    private void SelectCount(int newCount)
    {
        _selectedCount = Mathf.Clamp(newCount, 1, _maxCount);

        _isUpdatingSlider = true;
        QuantitySlider.value = _selectedCount;
        _isUpdatingSlider = false;

        UpdateDisplay();
    }

    private void UpdateMaxCount(int newMax)
    {
        _maxCount = Mathf.Clamp(newMax, 1, 99);

        if (_selectedCount > _maxCount)
            SelectCount(_maxCount);
        else
            UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        QuantityText.text = $"{_selectedCount} / {_maxCount}";
        TotalPriceText.text = $"필요한 골드: {TotalPrice}      <sprite name=Coin>";
    }

    // === 버튼 및 UI 이벤트 ===

    public void OnSliderValueChanged(float value)
    {
        if (_isUpdatingSlider) return;
        SelectCount(Mathf.RoundToInt(value));
    }

    public void OnClickPlusButton()
    {
        SelectCount(_selectedCount + 1);
    }

    public void OnClickMinusButton()
    {
        SelectCount(_selectedCount - 1);
    }

    public void OnClickBuyButton()
    {
        int totalPrice = TotalPrice;
        int selectedCount = _selectedCount;
        Debug.Log($"{_itemProfile.ItemDefinition.Name} {_selectedCount}개 구매 시도");

        if (!CurrencyManager.Instance.TrySpend(ECurrencyType.Gold, totalPrice))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        Debug.Log($"{_itemProfile.ItemDefinition.Name} {_selectedCount}개 구매 완료");
        var newItemInstance = new ItemInstance(_itemProfile, selectedCount);
        UnifiedInventoryManager.Instance.AddItem(newItemInstance);
        MasterAudio.PlaySound("Buy");
    }
}