using System.Collections.Generic;
using UnityEngine;

public class UI_GraphicSetting : MonoBehaviour
{
    [SerializeField]
    private TextSelectionSlider _fullscreenSlider;
    [SerializeField]
    private TextSelectionSlider _resolutionSlider;
    [SerializeField]
    private TextSelectionSlider _frameRateSlider;
    [SerializeField]
    private SettingData _settingData;

    private void OnEnable()
    {
        // 옵션 초기화 (올바른 매핑을 위해)
        _fullscreenSlider.Options = new List<string> { "전체화면", "창 모드", "테두리 없는 창" };
        _resolutionSlider.Options = new List<string> { "1920x1080 (FHD)", "2560x1440 (QHD)" };
        _frameRateSlider.Options = new List<string> { "30", "60", "120" };

        // 저장값 로드 & 적용
        _settingData.Load();
        _settingData.Apply();

        // UI 초기값 맞추기
        _fullscreenSlider.OptionText.text = _fullscreenSlider.Options[(int)_settingData.FullScreenMode];
        _resolutionSlider.OptionText.text = _settingData.Resolution.y == 1440
            ? _resolutionSlider.Options[1]
            : _resolutionSlider.Options[0];
        _frameRateSlider.OptionText.text = _settingData.FrameRate == 120 ? _frameRateSlider.Options[2] :
            _settingData.FrameRate == 60 ? _frameRateSlider.Options[1] : _frameRateSlider.Options[0];

        // 리스너 등록
        _fullscreenSlider.OnValueChanged += OnFullscreenChanged;
        _resolutionSlider.OnValueChanged += OnResolutionChanged;
        _frameRateSlider.OnValueChanged += OnFramerateChanged;
    }

    private void OnDisable()
    {
        _fullscreenSlider.OnValueChanged -= OnFullscreenChanged;
        _resolutionSlider.OnValueChanged -= OnResolutionChanged;
        _frameRateSlider.OnValueChanged -= OnFramerateChanged;
    }

    private void OnFullscreenChanged(int index)
    {
        _settingData.FullScreenMode = index switch
        {
            0 => FullScreenMode.FullScreenWindow,    // 전체화면
            1 => FullScreenMode.Windowed,            // 창 모드
            2 => FullScreenMode.ExclusiveFullScreen, // 테두리 없는 창
            _ => FullScreenMode.FullScreenWindow
        };
        _settingData.Apply();
        _settingData.Save();
    }

    private void OnResolutionChanged(int index)
    {
        _settingData.Resolution = index switch
        {
            1 => new Vector2Int(2560, 1440), // QHD
            _ => new Vector2Int(1920, 1080), // 기본 FHD
        };
        _settingData.Apply();
        _settingData.Save();
    }

    private void OnFramerateChanged(int index)
    {
        _settingData.FrameRate = index switch
        {
            2 => 120,
            1 => 60,
            _ => 30,
        };
        _settingData.Apply();
        _settingData.Save();
    }
}