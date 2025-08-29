using UnityEngine;

// 씬 전환 시 설정을 최신화 해주는 컴포넌트
public class SettingsApplier : MonoBehaviour
{
    [SerializeField] private SettingData _settingData;

    private void Awake()
    {
        if (_settingData == null)
        {
            Debug.LogWarning("[SettingsApplier] SettingData가 비어있습니다.");
            return;
        }

        var facade = new SettingsFacade(_settingData);
        facade.LoadAll();
        facade.ApplyAll();
    }
}