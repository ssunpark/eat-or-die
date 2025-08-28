using UnityEngine;

public class UISettingsResetButton : MonoBehaviour
{
    [SerializeField] private SettingData _settingData;
    [SerializeField] private UI_AudioSetting _audioUI;     // 선택
    [SerializeField] private UI_GraphicSetting _graphicUI; // 선택

    public void OnClickReset()
    {
        if (_settingData == null) return;

        _settingData.ResetToDefaults();

        // 옵션 목록/텍스트/슬라이더를 현재 데이터로 새로 그리기
        _audioUI?.RefreshUIFromData();
        _graphicUI?.RefreshUIFromData();

        _settingData.Apply();
        _settingData.Save();
        
        UI_Notification.Notify("기본값으로 설정되었습니다.");
    }
}