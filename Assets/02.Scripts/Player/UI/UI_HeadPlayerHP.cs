using UnityEngine;
using UnityEngine.UI;

public class UI_HeadPlayerHP : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Slider _hungerSlider;
    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Player reference is not set in UI_HeadPlayerHP.");
            return;
        }
        _player.Resource.OnHungerChanged += Resource_OnHungerChanged;
        Resource_OnHungerChanged(_player.Resource.CurrentHunger, _player.Resource.MaxHunger);
    }

    private void Resource_OnHungerChanged(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0)
        {
            Debug.LogWarning("Max health is zero or negative, setting slider value to 0.");
            _hungerSlider.value = 1; // MaxHealth가 0 이하인 경우 슬라이더 값을 0으로 설정
        }
        else
        {
            // 슬라이더 값 업데이트
            _hungerSlider.value = currentHealth / maxHealth;
        }
    }


}
