using DarkTonic.MasterAudio;
using UnityEngine;

public sealed class AudioSettingsService : IAudioSettingsService
{
    public void Apply(SettingData data)
    {
        MasterAudio.MasterVolumeLevel = Mathf.Clamp01(data.SfxVolume);
        MasterAudio.PlaylistMasterVolume = Mathf.Clamp01(data.MusicVolume);
    }
}