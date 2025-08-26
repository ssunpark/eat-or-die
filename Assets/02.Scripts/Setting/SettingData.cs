using UnityEngine;
using DarkTonic.MasterAudio;

[CreateAssetMenu(menuName = "Game/SettingData")]
public class SettingData : ScriptableObject
{
    // 오디오
    private const string KEY_SFX = "SETTINGS_SFX_VOLUME";
    private const string KEY_MUSIC = "SETTINGS_MUSIC_VOLUME";
    // 그래픽
    private const string KEY_FULLSCREEN_MODE = "SETTINGS_FULLSCREEN_MODE";
    private const string KEY_RESOLUTION = "SETTINGS_RESOLUTION";
    private const string KEY_FRAMERATE = "SETTINGS_FRAMERATE";
    
    // 오디오
    [Range(0f, 1f)]
    public float SfxVolume = 1f; // 0~1
    [Range(0f, 1f)]
    public float MusicVolume = 1f; // 0~1
    // 그래픽
    public FullScreenMode FullScreenMode = FullScreenMode.FullScreenWindow;
    public Vector2Int Resolution = new Vector2Int(1920, 1080); // 기본 FHD
    public int FrameRate = 60;                                 // 기본 60 FPS

    public void Load()
    {
        // 오디오
        SfxVolume = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : 1f;
        MusicVolume = PlayerPrefs.HasKey(KEY_MUSIC) ? PlayerPrefs.GetFloat(KEY_MUSIC) : 1f;
        // 그래픽
        FullScreenMode = (FullScreenMode)PlayerPrefs.GetInt(KEY_FULLSCREEN_MODE, (int)FullScreenMode.FullScreenWindow);
        int width = PlayerPrefs.GetInt(KEY_RESOLUTION + "_W", 1920);
        int height = PlayerPrefs.GetInt(KEY_RESOLUTION + "_H", 1080);
        Resolution = new Vector2Int(width, height);
        FrameRate = PlayerPrefs.GetInt(KEY_FRAMERATE, 60);
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
    public static float LinearToSlider(float linear0to1) => Mathf.Clamp01(linear0to1) * 100f;
}