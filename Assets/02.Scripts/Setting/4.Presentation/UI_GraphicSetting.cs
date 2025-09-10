using System.Linq;
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

    private SettingsFacade _facade;

    private void OnEnable()
    {
        _facade = new SettingsFacade(_settingData);

        // 저장값 로드
        _facade.LoadAll();

        // 옵션 구성
        _fullscreenSlider.Options = new() { "전체화면", "창 모드", "테두리 없는 창" };

        var resos = _facade.GetResolutions();
        _resolutionSlider.Options = resos.Select(_facade.ToResolutionLabel).ToList();

        _frameRateSlider.Options = _facade.GetFrameRateOptions().ToList();

        // 저장된 값 보정 & UI 초기화
        _settingData.Resolution = _facade.ClampResolution(_settingData.Resolution);
        _fullscreenSlider.Init(_facade.FullscreenModeToText(_settingData.FullScreenMode));
        _resolutionSlider.Init(_facade.ToResolutionLabel(_settingData.Resolution));
        _frameRateSlider.Init(_settingData.FrameRate.ToString());

        // 이벤트
        _fullscreenSlider.OnValueChanged += OnFullscreenChanged;
        _resolutionSlider.OnValueChanged += OnResolutionChanged;
        _frameRateSlider.OnValueChanged += OnFramerateChanged;

        // 적용
        _facade.ApplyGraphics();
    }

    private void OnDisable()
    {
        _fullscreenSlider.OnValueChanged -= OnFullscreenChanged;
        _resolutionSlider.OnValueChanged -= OnResolutionChanged;
        _frameRateSlider.OnValueChanged -= OnFramerateChanged;
    }

    private void OnFullscreenChanged(int index)
    {
        _settingData.FullScreenMode = _facade.TextIndexToFullscreenMode(index);
        _facade.ApplyGraphics();
        _facade.SaveAll();
    }

    private void OnResolutionChanged(int index)
    {
        var list = _facade.GetResolutions();
        if (index < 0 || index >= list.Count)
            return;

        _settingData.Resolution = list[index];
        _facade.ApplyGraphics();
        _facade.SaveAll();
    }

    private void OnFramerateChanged(int index)
    {
        _settingData.FrameRate = _facade.IndexToFrameRate(index);
        _facade.ApplyGraphics();
        _facade.SaveAll();
    }

    public void RefreshUIFromData()
    {
        if (_settingData == null)
            return;

        var resos = _facade.GetResolutions();
        _resolutionSlider.Options = resos.Select(_facade.ToResolutionLabel).ToList();

        _fullscreenSlider.OptionText.text = _facade.FullscreenModeToText(_settingData.FullScreenMode);
        _resolutionSlider.OptionText.text = _facade.ToResolutionLabel(_settingData.Resolution);
        _frameRateSlider.OptionText.text = _settingData.FrameRate.ToString();
    }
}