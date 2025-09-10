using UnityEngine;

// 강제로 늦게 실행
[DefaultExecutionOrder(10000)]
public class Letterboxer : MonoBehaviour
{
    [SerializeField]
    private SettingData _settingData;
    [SerializeField, Tooltip("비우면 MainCamera 사용")]
    private Camera _targetCamera; // 비우면 MainCamera 사용

    private Camera Cam => _targetCamera != null ? _targetCamera : Camera.main;

    void LateUpdate()
    {
        if (Cam == null || _settingData == null)
            return;

        // 창 모드에서만 레터/필러박스 적용
        if (_settingData.FullScreenMode != FullScreenMode.Windowed)
        {
            if (Cam.rect != new Rect(0f, 0f, 1f, 1f))
                Cam.rect = new Rect(0f, 0f, 1f, 1f);
            return;
        }

        float targetAspect = (float)_settingData.Resolution.x / _settingData.Resolution.y;
        float windowAspect = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(targetAspect, windowAspect))
        {
            if (Cam.rect != new Rect(0f, 0f, 1f, 1f))
                Cam.rect = new Rect(0f, 0f, 1f, 1f);
            return;
        }

        if (windowAspect > targetAspect)
        {
            // 창이 더 넓음 → 좌우 Pillarbox
            float scale = targetAspect / windowAspect; // 가시 영역의 너비 비율
            float x = (1f - scale) * 0.5f;
            Cam.rect = new Rect(x, 0f, scale, 1f);
        }
        else
        {
            // 창이 더 좁음/높음 → 위아래 Letterbox
            float scale = windowAspect / targetAspect; // 가시 영역의 높이 비율
            float y = (1f - scale) * 0.5f;
            Cam.rect = new Rect(0f, y, 1f, scale);
        }
    }
}