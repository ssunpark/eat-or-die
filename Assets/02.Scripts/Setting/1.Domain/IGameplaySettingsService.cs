public interface IGameplaySettingsService
{
    void Apply(SettingData data);
    void SetCameraShakeEnabled(bool enable);
    bool GetCameraShakeEnabled();
}