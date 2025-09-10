public interface ISettingsRepository
{
    void LoadInto(SettingData data);
    void SaveFrom(SettingData data);
}