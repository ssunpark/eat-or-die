using Unity.Cinemachine;

public sealed class GameplaySettingsService : IGameplaySettingsService
{
    private CinemachineImpulseListener _cached;

    private CinemachineImpulseListener GetListener()
    {
        if (_cached != null)
            return _cached;
        var vcam = UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
            _cached = vcam.GetComponent<CinemachineImpulseListener>();
        return _cached;
    }

    public void Apply(SettingData data)
    {
        var l = GetListener();
        if (l != null)
            l.enabled = data.CameraShakeEnabled;
    }

    public void SetCameraShakeEnabled(bool enable)
    {
        var l = GetListener();
        if (l != null)
            l.enabled = enable;
    }

    public bool GetCameraShakeEnabled()
    {
        var l = GetListener();
        return l != null && l.enabled;
    }
}