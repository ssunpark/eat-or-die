using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_HUDPlayerHP : MonoBehaviour
{
    // 플레이어의 배고픔 UI를 업데이트하는 스크립트
    [SerializeField] private Slider _hpSlider; // 배고픔 슬라이더 UI
    [SerializeField] private TextMeshProUGUI _hpText; // 배고픔 텍스트 UI

    private ResourceManager _resourceManager; // 리소스 매니저 인스턴스


    public void Initialize(ResourceManager resourceManager, StatManager statManager)
    {
        if (_resourceManager != null)
        {
            _resourceManager.OnHealthChanged -= _resourceManager_OnSatietyChanged;
        }

        _resourceManager = resourceManager;

        if (_resourceManager != null && statManager != null)
        {
            _resourceManager_OnSatietyChanged(_resourceManager.CurrentHealth, statManager.GetStat(EStatType.MaxHealth));
            // 이벤트 구독
            _resourceManager.OnHealthChanged += _resourceManager_OnSatietyChanged;
        }
        else
        {
            Debug.LogError("ResourceManager 또는 StatManager가 초기화되지 않았습니다.");
        }
        _resourceManager.OnSatietyChanged += _resourceManager_OnSatietyChanged;
    }

    private void _resourceManager_OnSatietyChanged(float currentHealth, float maxHealth)
    {
        // 배고픔 UI 업데이트
        if (_hpSlider != null)
        {
            _hpSlider.value = currentHealth / maxHealth; // 슬라이더 값 업데이트
        }
        if (_hpText != null)
        {
            _hpText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks when the HUD element is destroyed
        if (_resourceManager != null)
        {
            _resourceManager.OnHealthChanged -= _resourceManager_OnSatietyChanged;
        }
    }
}
