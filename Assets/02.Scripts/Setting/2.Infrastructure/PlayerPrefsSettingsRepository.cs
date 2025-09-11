using UnityEngine;

public sealed class PlayerPrefsSettingsRepository : ISettingsRepository
{
    // 키
    private const string KEY_SFX = "SETTINGS_SFX_VOLUME";
    private const string KEY_MUSIC = "SETTINGS_MUSIC_VOLUME";
    private const string KEY_FULLSCREEN = "SETTINGS_FULLSCREEN_MODE";
    private const string KEY_RESOLUTION_W = "SETTINGS_RESOLUTION_W";
    private const string KEY_RESOLUTION_H = "SETTINGS_RESOLUTION_H";
    private const string KEY_FRAMERATE = "SETTINGS_FRAMERATE";
    private const string KEY_CAMERA_SHAKE = "SETTINGS_CAMERA_SHAKE";

    public void LoadInto(SettingData d)
    {
        // 오디오
        d.SfxVolume = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : SettingData.DEFAULT_SFX;
        d.MusicVolume = PlayerPrefs.HasKey(KEY_MUSIC) ? PlayerPrefs.GetFloat(KEY_MUSIC) : SettingData.DEFAULT_MUSIC;

        // 그래픽
        d.FullScreenMode = PlayerPrefs.HasKey(KEY_FULLSCREEN)
            ? (FullScreenMode)PlayerPrefs.GetInt(KEY_FULLSCREEN)
            : SettingData.DEFAULT_FULLSCREEN;

        int w = PlayerPrefs.HasKey(KEY_RESOLUTION_W)
            ? PlayerPrefs.GetInt(KEY_RESOLUTION_W)
            : SettingData.DEFAULT_RESOLUTION.x;
        int h = PlayerPrefs.HasKey(KEY_RESOLUTION_H)
            ? PlayerPrefs.GetInt(KEY_RESOLUTION_H)
            : SettingData.DEFAULT_RESOLUTION.y;
        d.Resolution = new Vector2Int(w, h);

        d.FrameRate = PlayerPrefs.HasKey(KEY_FRAMERATE)
            ? PlayerPrefs.GetInt(KEY_FRAMERATE)
            : SettingData.DEFAULT_FRAMERATE;

        // 게임플레이
        d.CameraShakeEnabled = PlayerPrefs.HasKey(KEY_CAMERA_SHAKE)
            ? PlayerPrefs.GetInt(KEY_CAMERA_SHAKE) != 0
            : SettingData.DEFAULT_CAMERA_SHAKE;
    }

    public void SaveFrom(SettingData d)
    {
        PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(d.SfxVolume));
        PlayerPrefs.SetFloat(KEY_MUSIC, Mathf.Clamp01(d.MusicVolume));

        PlayerPrefs.SetInt(KEY_FULLSCREEN, (int)d.FullScreenMode);
        PlayerPrefs.SetInt(KEY_RESOLUTION_W, d.Resolution.x);
        PlayerPrefs.SetInt(KEY_RESOLUTION_H, d.Resolution.y);
        PlayerPrefs.SetInt(KEY_FRAMERATE, d.FrameRate);

        PlayerPrefs.SetInt(KEY_CAMERA_SHAKE, d.CameraShakeEnabled ? 1 : 0);

        PlayerPrefs.Save();
    }
}