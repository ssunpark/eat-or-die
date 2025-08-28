using UnityEngine;
using DarkTonic.MasterAudio;

[CreateAssetMenu(menuName = "Game/SettingData")]
public class SettingData : ScriptableObject
{
    // 기본값
    public static readonly float DEFAULT_SFX = 1f;
    public static readonly float DEFAULT_MUSIC = 1f;
    public static readonly FullScreenMode DEFAULT_FULLSCREEN = FullScreenMode.ExclusiveFullScreen;
    public static readonly Vector2Int DEFAULT_RESOLUTION = new(1920, 1080);
    public static readonly int DEFAULT_FRAMERATE = 60;

    // 오디오
    private const string KEY_SFX = "SETTINGS_SFX_VOLUME";
    private const string KEY_MUSIC = "SETTINGS_MUSIC_VOLUME";
    // 그래픽
    private const string KEY_FULLSCREEN_MODE = "SETTINGS_FULLSCREEN_MODE";
    private const string KEY_RESOLUTION = "SETTINGS_RESOLUTION";
    private const string KEY_FRAMERATE = "SETTINGS_FRAMERATE";

    // 오디오
    [Range(0f, 1f)]
    public float SfxVolume = DEFAULT_SFX; // 0~1
    [Range(0f, 1f)]
    public float MusicVolume = DEFAULT_MUSIC; // 0~1
    // 그래픽
    public FullScreenMode FullScreenMode = DEFAULT_FULLSCREEN;
    public Vector2Int Resolution = DEFAULT_RESOLUTION;
    public int FrameRate = DEFAULT_FRAMERATE;

    public void Load()
    {
        // 오디오
        SfxVolume = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : DEFAULT_SFX;
        MusicVolume = PlayerPrefs.HasKey(KEY_MUSIC) ? PlayerPrefs.GetFloat(KEY_MUSIC) : DEFAULT_MUSIC;

        // 그래픽
        FullScreenMode = PlayerPrefs.HasKey(KEY_FULLSCREEN_MODE)
            ? (FullScreenMode)PlayerPrefs.GetInt(KEY_FULLSCREEN_MODE)
            : DEFAULT_FULLSCREEN;

        Resolution = (PlayerPrefs.HasKey(KEY_RESOLUTION + "_W") && PlayerPrefs.HasKey(KEY_RESOLUTION + "_H"))
            ? new Vector2Int(
                PlayerPrefs.GetInt(KEY_RESOLUTION + "_W"),
                PlayerPrefs.GetInt(KEY_RESOLUTION + "_H"))
            : DEFAULT_RESOLUTION;

        FrameRate = PlayerPrefs.HasKey(KEY_FRAMERATE)
            ? PlayerPrefs.GetInt(KEY_FRAMERATE)
            : DEFAULT_FRAMERATE;
    }

    public void Save()
    {
        // 오디오
        PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(SfxVolume));
        PlayerPrefs.SetFloat(KEY_MUSIC, Mathf.Clamp01(MusicVolume));
        // 그래픽
        PlayerPrefs.SetInt(KEY_FULLSCREEN_MODE, (int)FullScreenMode);
        PlayerPrefs.SetInt(KEY_RESOLUTION + "_W", Resolution.x);
        PlayerPrefs.SetInt(KEY_RESOLUTION + "_H", Resolution.y);
        PlayerPrefs.SetInt(KEY_FRAMERATE, FrameRate);
        PlayerPrefs.Save();
    }

    public void Apply()
    {
        // 오디오
        MasterAudio.MasterVolumeLevel = Mathf.Clamp01(SfxVolume);
        MasterAudio.PlaylistMasterVolume = Mathf.Clamp01(MusicVolume);
        // 그래픽
        Screen.SetResolution(Resolution.x, Resolution.y, FullScreenMode);
        Application.targetFrameRate = FrameRate;
    }

    public static float SliderToLinear(float value) => Mathf.Clamp01(value / 100f);
    public static float LinearToSlider(float linear) => Mathf.Clamp01(linear) * 100f;

    public void ResetToDefaults()
    {
        SfxVolume = DEFAULT_SFX;
        MusicVolume = DEFAULT_MUSIC;
        FullScreenMode = DEFAULT_FULLSCREEN;
        Resolution = DEFAULT_RESOLUTION;
        FrameRate = DEFAULT_FRAMERATE;
    }
}