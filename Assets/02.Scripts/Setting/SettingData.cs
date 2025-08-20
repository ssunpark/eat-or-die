using UnityEngine;
using DarkTonic.MasterAudio;

[CreateAssetMenu(menuName = "Game/SettingData")]
public class SettingData : ScriptableObject
{
    private const string KEY_SFX = "SETTINGS_SFX_VOLUME";
    private const string KEY_MUSIC = "SETTINGS_MUSIC_VOLUME";

    [Range(0f, 1f)]
    public float sfxVolume = 1f; // 0~1
    [Range(0f, 1f)]
    public float musicVolume = 1f; // 0~1

    public void Load()
    {
        sfxVolume = PlayerPrefs.HasKey(KEY_SFX) ? PlayerPrefs.GetFloat(KEY_SFX) : 1f;
        musicVolume = PlayerPrefs.HasKey(KEY_MUSIC) ? PlayerPrefs.GetFloat(KEY_MUSIC) : 1f;
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(KEY_SFX, Mathf.Clamp01(sfxVolume));
        PlayerPrefs.SetFloat(KEY_MUSIC, Mathf.Clamp01(musicVolume));
        PlayerPrefs.Save();
    }

    public void Apply()
    {
        MasterAudio.MasterVolumeLevel = Mathf.Clamp01(sfxVolume);
        MasterAudio.PlaylistMasterVolume = Mathf.Clamp01(musicVolume);
    }
    
    public static float SliderToLinear(float value) => Mathf.Clamp01(value / 100f);
    public static float LinearToSlider(float linear0to1) => Mathf.Clamp01(linear0to1) * 100f;
}