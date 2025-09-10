using System.Collections.Generic;
using UnityEngine;

public interface IGraphicsSettingsService
{
    void Apply(SettingData data);

    IReadOnlyList<Vector2Int> GetFilteredResolutions();
    Vector2Int ClampToMonitor(Vector2Int r);

    string FullscreenModeToText(FullScreenMode mode);
    FullScreenMode TextIndexToFullscreenMode(int index);

    string ToResolutionLabel(Vector2Int r);

    IReadOnlyList<string> GetFrameRateOptions();
    int IndexToFrameRate(int index);
}