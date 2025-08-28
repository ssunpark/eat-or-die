using System.Collections.Generic;
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

    // 내부용: 실제 선택 가능한 해상도 목록
    private readonly List<Vector2Int> _candidateResolutions = new List<Vector2Int>
    {
        new(1280, 720),  // HD
        new(1920, 1080), // FHD
        new(2560, 1440), // QHD
    };

    private List<Vector2Int> _filteredResolutions; // 모니터에 맞춰 필터링된 결과

    private void OnEnable()
    {
        // 저장값 로드
        _settingData.Load();      // Apply는 UI 초기화 완료 후에 호출함
        BuildFullscreenOptions(); // 전체화면 옵션 텍스트 구성
        BuildResolutionOptions(); // 모니터 해상도 기준으로 옵션 필터링 및 구성
        BuildFramerateOptions();  // 프레임 옵션 텍스트 구성

        // UI 텍스트 초기 세팅
        _fullscreenSlider.Init(FullscreenModeToText(_settingData.FullScreenMode));
        _resolutionSlider.Init(ResolutionToText(ClampResolutionToMonitor(_settingData.Resolution)));
        _frameRateSlider.Init(_settingData.FrameRate.ToString());

        // 리스너
        _fullscreenSlider.OnValueChanged += OnFullscreenChanged;
        _resolutionSlider.OnValueChanged += OnResolutionChanged;
        _frameRateSlider.OnValueChanged += OnFramerateChanged;

        // 최종 적용
        _settingData.Apply();
    }

    private void OnDisable()
    {
        _fullscreenSlider.OnValueChanged -= OnFullscreenChanged;
        _resolutionSlider.OnValueChanged -= OnResolutionChanged;
        _frameRateSlider.OnValueChanged -= OnFramerateChanged;
    }

    private void BuildFullscreenOptions()
    {
        // 실제 의미에 맞춘 라벨
        _fullscreenSlider.Options = new List<string> { "전체화면", "창 모드", "테두리 없는 창" };
    }

    private string FullscreenModeToText(FullScreenMode mode)
    {
        // enum 값과 텍스트 매핑
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                return "전체화면";
            case FullScreenMode.Windowed:
                return "창 모드";
            case FullScreenMode.FullScreenWindow:
                return "테두리 없는 창";
            default:
                return "테두리 없는 창";
        }
    }

    private FullScreenMode TextToFullscreenMode(int index)
    {
        // 0: 전체화면(Exclusive), 1: 창 모드(Windowed), 2: 테두리 없는 창(FullScreenWindow)
        return index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.Windowed,
            2 => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.FullScreenWindow
        };
    }

    private void BuildResolutionOptions()
    {
        int monW = Display.main.systemWidth;
        int monH = Display.main.systemHeight;

        // 모니터보다 큰 해상도는 제외
        _filteredResolutions = _candidateResolutions
            .Where(r => r.x <= monW && r.y <= monH)
            .OrderBy(r => r.x * r.y)
            .ToList();

        // 최소 1개는 있어야 함 (모니터가 너무 작아도 HD가 들어가도록 안전장치)
        if (_filteredResolutions.Count == 0)
            _filteredResolutions.Add(new Vector2Int(Mathf.Min(1280, monW), Mathf.Min(720, monH)));

        _resolutionSlider.Options = _filteredResolutions
            .Select(ResolutionToText)
            .ToList();

        // 저장된 해상도가 필터를 벗어나면 가장 가까운(면적 기준) 값으로 보정
        _settingData.Resolution = ClampResolutionToMonitor(_settingData.Resolution);
    }

    private string ResolutionToText(Vector2Int r)
    {
        string label = (r.x, r.y) switch
        {
            (1280, 720) => " (HD)",
            (1920, 1080) => " (FHD)",
            (2560, 1440) => " (QHD)",
            _ => ""
        };
        return $"{r.x}x{r.y}{label}";
    }

    private Vector2Int ClampResolutionToMonitor(Vector2Int r)
    {
        int monW = Display.main.systemWidth;
        int monH = Display.main.systemHeight;
        if (r.x <= monW && r.y <= monH && _filteredResolutions.Contains(r))
            return r;

        // 가장 면적이 비슷한 후보로 스냅
        return _filteredResolutions
            .OrderBy(c => Mathf.Abs(c.x * c.y - r.x * r.y))
            .First();
    }

    private void BuildFramerateOptions()
    {
        _frameRateSlider.Options = new List<string> { "30", "60", "120" };
        if (!_frameRateSlider.Options.Contains(_settingData.FrameRate.ToString()))
            _settingData.FrameRate = 60;
    }

    private void OnFullscreenChanged(int index)
    {
        _settingData.FullScreenMode = TextToFullscreenMode(index);
        _settingData.Apply();
        _settingData.Save();
    }

    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= _filteredResolutions.Count)
            return;

        _settingData.Resolution = _filteredResolutions[index];
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
    
    public void RefreshUIFromData()
    {
        if (_settingData == null) return;

        // 옵션 목록 재구성 (모니터 크기 기준 필터)
        BuildFullscreenOptions();
        BuildResolutionOptions();
        BuildFramerateOptions();

        // 현재 데이터 → UI 텍스트만 반영 (적용/저장 X)
        _fullscreenSlider.OptionText.text = FullscreenModeToText(_settingData.FullScreenMode);
        _resolutionSlider.OptionText.text = ResolutionToText(_settingData.Resolution);
        _frameRateSlider.OptionText.text = _settingData.FrameRate.ToString();
    }
}