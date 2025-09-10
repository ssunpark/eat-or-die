using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GraphicsSettingsService : IGraphicsSettingsService
{
    private static readonly List<Vector2Int> _candidates = new()
    {
        new(1280, 720),  // HD
        new(1920, 1080), // FHD
        new(2560, 1440)  // QHD
    };

    private List<Vector2Int> _filtered;

    public void Apply(SettingData data)
    {
        Screen.SetResolution(data.Resolution.x, data.Resolution.y, data.FullScreenMode);
        Application.targetFrameRate = data.FrameRate;
    }

    public IReadOnlyList<Vector2Int> GetFilteredResolutions()
    {
        if (_filtered != null)
            return _filtered;

        int monW = Display.main.systemWidth;
        int monH = Display.main.systemHeight;

        _filtered = _candidates
            .Where(r => r.x <= monW && r.y <= monH)
            .OrderBy(r => r.x * r.y)
            .ToList();

        if (_filtered.Count == 0)
            _filtered.Add(new Vector2Int(Mathf.Min(1280, monW), Mathf.Min(720, monH)));

        return _filtered;
    }

    public Vector2Int ClampToMonitor(Vector2Int r)
    {
        var list = GetFilteredResolutions();
        int monW = Display.main.systemWidth;
        int monH = Display.main.systemHeight;

        if (r.x <= monW && r.y <= monH && list.Contains(r))
            return r;

        return list.OrderBy(c => Mathf.Abs(c.x * c.y - r.x * r.y)).First();
    }

    public string FullscreenModeToText(FullScreenMode mode) => mode switch
    {
        FullScreenMode.ExclusiveFullScreen => "전체화면",
        FullScreenMode.Windowed => "창 모드",
        FullScreenMode.FullScreenWindow => "테두리 없는 창",
        _ => "테두리 없는 창"
    };

    public FullScreenMode TextIndexToFullscreenMode(int index) => index switch
    {
        0 => FullScreenMode.ExclusiveFullScreen,
        1 => FullScreenMode.Windowed,
        2 => FullScreenMode.FullScreenWindow,
        _ => FullScreenMode.FullScreenWindow
    };

    public string ToResolutionLabel(Vector2Int r)
    {
        string tag = (r.x, r.y) switch
        {
            (1280, 720) => " (HD)",
            (1920, 1080) => " (FHD)",
            (2560, 1440) => " (QHD)",
            _ => ""
        };
        return $"{r.x}x{r.y}{tag}";
    }

    public IReadOnlyList<string> GetFrameRateOptions() => new List<string> { "30", "60", "120" };

    public int IndexToFrameRate(int index) => index switch
    {
        2 => 120,
        1 => 60,
        _ => 30
    };
}