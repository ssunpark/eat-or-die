using UnityEngine;

public class UISettingsResetButton : MonoBehaviour
{
    [SerializeField] private SettingData _settingData;
    [SerializeField] private UI_AudioSetting    _audioUI;    // 선택
    [SerializeField] private UI_GraphicSetting  _graphicUI;  // 선택
    [SerializeField] private UI_GamePlaySetting _gameplayUI; // 선택

    public void OnClickReset()
    {
        if (_settingData == null) return;

        var facade = new SettingsFacade(_settingData);

        _settingData.ResetToDefaults();

        // UI 새로고침
        _audioUI?.RefreshUIFromData();
        _graphicUI?.RefreshUIFromData();
        _gameplayUI?.RefreshUIFromData();

        // 적용 & 저장
        facade.ApplyAll();
        facade.SaveAll();

        UI_Notification.Notify("기본값으로 설정되었습니다.");
    }
}