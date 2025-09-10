using UnityEngine;
using UnityEngine.UI;

public class UI_GamePlaySetting : MonoBehaviour
{
    [SerializeField]
    private Slider _shakeSlider; // 0~1 (정수)
    [SerializeField]
    private SettingData _settingData;

    private SettingsFacade _facade;

    private void OnEnable()
    {
        _facade = new SettingsFacade(_settingData);

        _shakeSlider.minValue = 0f;
        _shakeSlider.maxValue = 1f;
        _shakeSlider.wholeNumbers = true;

        _facade.LoadAll();       // 최신값 로드
        _facade.ApplyGameplay(); // 적용

        // UI 반영
        _shakeSlider.SetValueWithoutNotify(_settingData.CameraShakeEnabled ? 1f : 0f);

        _shakeSlider.onValueChanged.AddListener(OnShakeValueChanged);
    }

    private void OnDisable()
    {
        _shakeSlider.onValueChanged.RemoveListener(OnShakeValueChanged);
    }

    private void OnShakeValueChanged(float value)
    {
        bool enable = Mathf.Approximately(value, 1f);
        _facade.SetCameraShakeEnabled(enable);
        _facade.SaveAll();
    }

    public void RefreshUIFromData()
    {
        _shakeSlider.SetValueWithoutNotify(_settingData.CameraShakeEnabled ? 1f : 0f);
    }
}