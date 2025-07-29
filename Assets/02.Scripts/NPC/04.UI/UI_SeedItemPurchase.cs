using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 재배 모종 아이템 세부사항의 동적 데이터 UI
public class UI_SeedItemPurchase : MonoBehaviour
{
    [Header("UI 연결")]
    public Slider QuantitySlider;
    public Button PlusButton;
    public Button MinusButton;
    public Button BuyButton;
    public TextMeshProUGUI QuantityText;
    public TextMeshProUGUI TotalPriceText;

    private AItemInfo _itemInfo;
    private NpcItem _npcItem;
    private int _selectedCount;
    private int _maxCount;
    private int _unitPrice;

    private bool _isUpdatingSlider = false;

    private void Start()
    {
        QuantitySlider.minValue = 1;
        QuantitySlider.maxValue = 99;
        QuantitySlider.wholeNumbers = true;

        QuantitySlider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
    }

    public void Init(AItemInfo itemInfo, NpcItem npcItem, int maxCount)
    {
        _itemInfo = itemInfo;
        _npcItem = npcItem;
        _unitPrice = npcItem.Price;
        _maxCount = Mathf.Clamp(maxCount, 1, 99);

        _selectedCount = 1;
        QuantitySlider.value = _selectedCount;
        UpdateQuantityUI();
    }

    public void OnSliderChanged()
    {
        if (_isUpdatingSlider) return;

        int rawValue = Mathf.RoundToInt(QuantitySlider.value);

        // 슬라이더는 99까지 가지만 실제 제한은 _maxCount까지
        int clampedValue = Mathf.Clamp(rawValue, 1, _maxCount);

        if (clampedValue != rawValue)
        {
            _isUpdatingSlider = true;
            QuantitySlider.value = clampedValue;
            _isUpdatingSlider = false;
        }

        _selectedCount = clampedValue;
        UpdateQuantityUI();
    }

    public void OnClickPlus()
    {
        if (_selectedCount < _maxCount)
        {
            _selectedCount++;
            QuantitySlider.value = _selectedCount;
        }
    }

    public void OnClickMinus()
    {
        if (_selectedCount > 1)
        {
            _selectedCount--;
            QuantitySlider.value = _selectedCount;
        }
    }

    private void UpdateQuantityUI()
    {
        QuantityText.text = $"{_selectedCount} / {_maxCount}";
        TotalPriceText.text = $"필요한 골드: {_unitPrice * _selectedCount}";
    }

    public void OnClickBuy()
    {
        Debug.Log($"{_itemInfo.ItemData.Name} {_selectedCount}개 구매 시도");
    }
}