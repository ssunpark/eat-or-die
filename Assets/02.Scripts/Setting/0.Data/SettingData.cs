using UnityEngine;

[CreateAssetMenu(menuName = "Game/SettingData")]
public class SettingData : ScriptableObject
{
    // 기본값
    public static readonly float DEFAULT_SFX = 1f;
    public static readonly float DEFAULT_MUSIC = 1f;
    public static readonly FullScreenMode DEFAULT_FULLSCREEN = FullScreenMode.ExclusiveFullScreen;
    public static readonly Vector2Int DEFAULT_RESOLUTION = new(1920, 1080);
    public static readonly int DEFAULT_FRAMERATE = 60;
    public static readonly bool DEFAULT_CAMERA_SHAKE = true;

    // 실제 값
    [Range(0f,1f)] public float SfxVolume = DEFAULT_SFX;
    [Range(0f,1f)] public float MusicVolume = DEFAULT_MUSIC;
    public FullScreenMode FullScreenMode = DEFAULT_FULLSCREEN;
    public Vector2Int    Resolution     = DEFAULT_RESOLUTION;
    public int           FrameRate      = DEFAULT_FRAMERATE;
    public bool          CameraShakeEnabled = DEFAULT_CAMERA_SHAKE;

    // 유틸
    public static float SliderToLinear(float value)  => Mathf.Clamp01(value / 100f);
    public static float LinearToSlider(float linear) => Mathf.Clamp01(linear) * 100f;

    public void ResetToDefaults()
    {
        SfxVolume = DEFAULT_SFX;
        MusicVolume = DEFAULT_MUSIC;
        FullScreenMode = DEFAULT_FULLSCREEN;
        Resolution = DEFAULT_RESOLUTION;
        FrameRate = DEFAULT_FRAMERATE;
        CameraShakeEnabled = DEFAULT_CAMERA_SHAKE;
    }
}