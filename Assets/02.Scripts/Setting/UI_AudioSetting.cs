using UnityEngine;
using UnityEngine.UI;

public class UI_AudioSetting : MonoBehaviour
{
    [SerializeField]
    private Slider _sfxSlider; // 0~100
    [SerializeField]
    private Slider _musicSlider; // 0~100
    [SerializeField]
    private SettingData _settingData; // SettingData.asset 드래그

    private void OnEnable()
    {
        // 안전 보정
        _sfxSlider.minValue = 0f;
        _sfxSlider.maxValue = 100f;
        _sfxSlider.wholeNumbers = true;
        _musicSlider.minValue = 0f;
        _musicSlider.maxValue = 100f;
        _musicSlider.wholeNumbers = true;

        // 저장값 로드 & 적용
        _settingData.Load();
        _settingData.Apply();

        // UI 초기화
        _sfxSlider.SetValueWithoutNotify(SettingData.LinearToSlider(_settingData.SfxVolume));
        _musicSlider.SetValueWithoutNotify(SettingData.LinearToSlider(_settingData.MusicVolume));

        // 리스너
        _sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    private void OnDisable()
    {
        _sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }

    private void OnSfxChanged(float value) // 0~100
    {
        _settingData.SfxVolume = SettingData.SliderToLinear(value);
        _settingData.Apply();
        _settingData.Save();
    }

    private void OnMusicChanged(float value) // 0~100
    {
        _settingData.MusicVolume = SettingData.SliderToLinear(value);
        _settingData.Apply();
        _settingData.Save();
    }
    
    public void RefreshUIFromData()
    {
        if (_settingData == null) return;
        _sfxSlider.SetValueWithoutNotify(SettingData.LinearToSlider(_settingData.SfxVolume));
        _musicSlider.SetValueWithoutNotify(SettingData.LinearToSlider(_settingData.MusicVolume));
    }
}