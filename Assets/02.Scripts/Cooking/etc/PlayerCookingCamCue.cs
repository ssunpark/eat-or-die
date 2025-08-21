using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCookingCamCue : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnCamCueChanged))]
    private NetworkBool _camCueActive { get; set; }  // 서버가 true/false 토글

    [Header("Cinemachine 3.1.4")]
    public CinemachineVirtualCameraBase CineCam; // 전용 VCam
    public int NormalPriority = 10;
    public int CinematicPriority = 200;

    [Header("Anchor (Optional)")]
    public Transform LookOffset;                    // 없으면 CloseUpOffset 사용
    [SerializeField] private string _anchorName = "CamOrbitAnchor";

    [Header("Orbit Start Fitting")]
    [SerializeField] private bool _matchCurrentViewOnEnter = true; // 진입 시 현재 뷰로 시드
    [SerializeField, Min(0f)] private float _fitDuration = 0.25f;

    [Header("Orbit")]
    public float OrbitDuration = 3f;                // 3초
    public float OrbitRadius = 3.5f;
    public float OrbitHeight = 1.6f;
    public float OrbitTurns = 1.5f;                   // 1 == 한 바퀴

    [Header("Close-Up")]
    public float CloseUpDuration = 1f;              // 1초
    public float CloseUpDistance = 1.2f;
    public Vector3 CloseUpOffset = new Vector3(0f, 0.5f, 0f);

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public override void Spawned()
    {
        base.Spawned();
        if (CineCam != null) CineCam.Priority = NormalPriority;
    }

    // === 서버에서만 호출: 카메라 큐 on/off ===
    public void ServerSetCamCue(bool active)
    {
        if (!HasStateAuthority) return;
        _camCueActive = active;
    }

    private void OnCamCueChanged()
    {
        // 로컬(내 입력 권한)만 카메라 연출
        if (!Object.HasInputAuthority) return;

        if (_camCueActive)
            RunCinematicAsync().Forget();
        else
            CancelIfRunning();
    }

    private async UniTaskVoid RunCinematicAsync()
    {
        if (_isRunning) CancelIfRunning();
        _isRunning = true;
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        var brain = Camera.main ? Camera.main.GetComponent<CinemachineBrain>() : null;

        // 기존 상태 백업
        int prevPriority = CineCam.Priority;
        var prevFollow = CineCam.Follow;
        var prevLookAt = CineCam.LookAt;

        // 0) "현재 뷰"로 시드해서 진입 스냅 제거
        if (_matchCurrentViewOnEnter)
        {
            var viewTr = brain?.OutputCamera != null ? brain.OutputCamera.transform : Camera.main?.transform;
            if (viewTr != null)
                CineCam.transform.SetPositionAndRotation(viewTr.position, viewTr.rotation);
        }

        // 연출용으로 직접 제어
        CineCam.Follow = null;
        CineCam.LookAt = null;

        // 우선순위 올려서 브레인 블렌드로 자연스럽게 전환
        CineCam.Priority = CinematicPriority;

        try
        {
            // 앵커 확보(스폰 지연 고려)
            Transform anchor = LookOffset != null ? LookOffset : await ResolveAnchorAsync(token);
            if (anchor == null) throw new System.Exception("Anchor not found.");

            // === ORBIT 시작값을 "현재 카메라"에서 추출 ===
            Vector3 center = anchor.position;

            // 현재 카메라 기준 벡터
            Vector3 init = CineCam.transform.position - center;
            float startHeight = init.y;

            Vector2 initXZ = new Vector2(init.x, init.z);
            float startRadius = initXZ.magnitude;

            // 시작 각도(도)
            float startAngle;
            if (startRadius < 0.01f)
            {
                // 너무 가까우면 앵커 정면을 기준으로 시작
                Vector3 fwd = anchor.forward.sqrMagnitude > 1e-4f ? anchor.forward : Vector3.forward;
                startAngle = Mathf.Atan2(fwd.z, fwd.x) * Mathf.Rad2Deg;
                startRadius = OrbitRadius;
                startHeight = OrbitHeight;
            }
            else
            {
                startAngle = Mathf.Atan2(initXZ.y, initXZ.x) * Mathf.Rad2Deg;
            }

            // === 1) ORBIT ===
            float t = 0f;
            while (t < OrbitDuration)
            {
                token.ThrowIfCancellationRequested();

                float ratio = t / OrbitDuration;                     // 0~1
                float angle = startAngle + ratio * OrbitTurns * 360f; // 시작각에서 이어서 회전

                // 초기 반지름/높이를 타깃으로 부드럽게 맞춰가기
                float fit = (_fitDuration > 0f)
                    ? Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / _fitDuration))
                    : 1f;

                float radius = Mathf.Lerp(startRadius, OrbitRadius, fit);
                float height = Mathf.Lerp(startHeight, OrbitHeight, fit);

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    height,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );

                Vector3 camPos = center + offset;
                CineCam.transform.position = camPos;

                Vector3 lookPoint = anchor.position + CloseUpOffset;
                Vector3 dir = (lookPoint - camPos);
                if (dir.sqrMagnitude > 1e-6f)
                    CineCam.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

                await UniTask.NextFrame(token);
                t += Time.deltaTime;
            }

            // === 2) CLOSE-UP === (그대로 유지)
            {
                Vector3 startPos = CineCam.transform.position;
                Quaternion startRot = CineCam.transform.rotation;

                Vector3 lookPoint = anchor.position + CloseUpOffset;
                Vector3 forward = anchor.forward.sqrMagnitude > 1e-4f ? anchor.forward : CineCam.transform.forward;

                Vector3 dstPos = lookPoint - forward * CloseUpDistance;
                Quaternion dstRot = Quaternion.LookRotation(lookPoint - dstPos, Vector3.up);

                float d = 0f;
                while (d < CloseUpDuration)
                {
                    token.ThrowIfCancellationRequested();

                    float r = d / CloseUpDuration;
                    CineCam.transform.position = Vector3.Lerp(startPos, dstPos, r);
                    CineCam.transform.rotation = Quaternion.Slerp(startRot, dstRot, r);

                    await UniTask.NextFrame(token);
                    d += Time.deltaTime;
                }
            }
        }
        catch (OperationCanceledException) { /* 취소 시 바로 원복 */ }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[PlayerCookingCamCue] {e.Message}");
        }
        finally
        {
            // 원복 (브레인 블렌드로 자연 복귀)
            CineCam.Follow = prevFollow;
            CineCam.LookAt = prevLookAt;
            CineCam.Priority = prevPriority;

            _cts?.Dispose();
            _cts = null;
            _isRunning = false;
        }
    }

    private void CancelIfRunning()
    {
        if (_isRunning && _cts != null && !_cts.IsCancellationRequested)
            _cts.Cancel();
    }

    private async UniTask<Transform> ResolveAnchorAsync(CancellationToken token)
    {
        // 1) 지정 이름 우선
        if (!string.IsNullOrEmpty(_anchorName))
        {
            var t = transform.Find(_anchorName) ?? FindDeep(transform, _anchorName);
            if (t != null) return t;
        }
        // 2) 대체 이름들
        string[] names = { "CameraAnchor", "Head" };
        foreach (var n in names)
        {
            var t = transform.Find(n) ?? FindDeep(transform, n);
            if (t != null) return t;
        }

        // 3) 못 찾았으면 루트 + 약간의 대기(스폰 지연 대비)
        float waited = 0f;
        while (waited < 1.0f) // 최대 1초만 대기
        {
            token.ThrowIfCancellationRequested();
            var t = transform.Find(_anchorName) ?? FindDeep(transform, _anchorName);
            if (t != null) return t;
            await UniTask.Delay(50, cancellationToken: token);
            waited += 0.05f;
        }
        return transform;

        static Transform FindDeep(Transform r, string name)
        {
            for (int i = 0; i < r.childCount; i++)
            {
                var c = r.GetChild(i);
                if (c.name == name) return c;
                var d = FindDeep(c, name);
                if (d != null) return d;
            }
            return null;
        }
    }
}
