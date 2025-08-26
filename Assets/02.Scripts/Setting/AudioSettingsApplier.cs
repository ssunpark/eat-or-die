using UnityEngine;

// 씬 전환 시 설정을 최신화 해주는 컴포넌트
public class AudioSettingsApplier : MonoBehaviour
{
    [SerializeField] private SettingData _settingData; // SettingData.asset 드래그

    private void Awake()
    {
        if (_settingData == null)
        {
            Debug.LogWarning("[AudioSettingsApplier] SettingData가 비어있습니다.");
            return;
        }

        // 씬 시작할 때마다 저장값 불러와서 적용
        _settingData.Load();
        _settingData.Apply();
    }
}