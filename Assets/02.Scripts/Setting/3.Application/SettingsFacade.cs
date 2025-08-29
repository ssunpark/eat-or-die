using System.Collections.Generic;
using UnityEngine;

public sealed class SettingsFacade
{
    private readonly SettingData _data;
    private readonly ISettingsRepository _repo;
    private readonly IAudioSettingsService _audio;
    private readonly IGraphicsSettingsService _gfx;
    private readonly IGameplaySettingsService _gameplay;

    public SettingsFacade(SettingData data)
    {
        _data = data;
        _repo = new PlayerPrefsSettingsRepository();
        _audio = new AudioSettingsService();
        _gfx = new GraphicsSettingsService();
        _gameplay = new GameplaySettingsService();
    }

    public void LoadAll() => _repo.LoadInto(_data);
    public void SaveAll() => _repo.SaveFrom(_data);

    public void ApplyAll()
    {
        _audio.Apply(_data);
        _gfx.Apply(_data);
        _gameplay.Apply(_data);
    }

    // 부분 적용
    public void ApplyAudio() => _audio.Apply(_data);
    public void ApplyGraphics() => _gfx.Apply(_data);
    public void ApplyGameplay() => _gameplay.Apply(_data);

    // UI 빠른 접근
    public IReadOnlyList<Vector2Int> GetResolutions()
        => _gfx.GetFilteredResolutions();

    public Vector2Int ClampResolution(Vector2Int r)
        => _gfx.ClampToMonitor(r);

    public string FullscreenModeToText(FullScreenMode mode)
        => _gfx.FullscreenModeToText(mode);

    public FullScreenMode TextIndexToFullscreenMode(int index)
        => _gfx.TextIndexToFullscreenMode(index);

    public string ToResolutionLabel(Vector2Int r)
        => _gfx.ToResolutionLabel(r);

    public IReadOnlyList<string> GetFrameRateOptions()
        => _gfx.GetFrameRateOptions();

    public int IndexToFrameRate(int index)
        => _gfx.IndexToFrameRate(index);

    public void SetCameraShakeEnabled(bool enable)
    {
        _data.CameraShakeEnabled = enable;
        _gameplay.SetCameraShakeEnabled(enable);
    }

    public bool GetCameraShakeEnabled() => _gameplay.GetCameraShakeEnabled();
}