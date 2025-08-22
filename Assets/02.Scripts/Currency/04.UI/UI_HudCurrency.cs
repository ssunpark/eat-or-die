using TMPro;
using UnityEngine;

public class UI_HudCurrency : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hudGoldText;

    private void Start()
    {
        CurrencyManager.Instance.OnCurrencyChanged += OnGoldAmountChanged;
        UpdateText(CurrencyManager.Instance.Get(ECurrencyType.Gold));
    }

    private void OnGoldAmountChanged(ECurrencyType currency, int newAmount)
    {
        UpdateText(newAmount);
    }

    private void UpdateText(int amount)
    {
        if (_hudGoldText != null)
        {
            _hudGoldText.text = amount.ToString();
        }
    }
}