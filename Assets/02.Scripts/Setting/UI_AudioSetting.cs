using UnityEngine;
using UnityEngine.UI;

public class UI_AudioSetting : MonoBehaviour
{
    [SerializeField]
    private Slider sfxSlider; // 0~100
    [SerializeField]
    private Slider musicSlider; // 0~100
    [SerializeField]
    private SettingData settingData; // SettingData.asset 드래그

    private void OnEnable()
    {
        // 안전 보정
        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 100f;
        sfxSlider.wholeNumbers = true;
        musicSlider.minValue = 0f;
        musicSlider.maxValue = 100f;
        musicSlider.wholeNumbers = true;

        // 저장값 로드 & 적용
        settingData.Load();
        settingData.Apply();

        // UI 초기화
        sfxSlider.SetValueWithoutNotify(SettingData.LinearToSlider(settingData.sfxVolume));
        musicSlider.SetValueWithoutNotify(SettingData.LinearToSlider(settingData.musicVolume));

        // 리스너
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    private void OnDisable()
    {
        sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }

    private void OnSfxChanged(float value) // 0~100
    {
        settingData.sfxVolume = SettingData.SliderToLinear(value);
        settingData.Apply();
        settingData.Save();
    }

    private void OnMusicChanged(float value) // 0~100
    {
        settingData.musicVolume = SettingData.SliderToLinear(value);
        settingData.Apply();
        settingData.Save();
    }
}