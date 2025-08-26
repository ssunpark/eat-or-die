using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cmCam;

    private PlayerInputActions _inputAction;
    private readonly List<Player> _targets = new();
    private readonly Dictionary<Transform, Vector3> _lastPos = new();
    private int _currentIndex = -1;


    public event Action<Transform, Player> SpectateTargetChanged;
    public Player CurrentSpectatedPlayer { get; private set; }

    [SerializeField] private float _teleportSqrThreshold = 25f;

    private void Start()
    {
        if (InputReader.Instance != null)
            _inputAction = InputReader.Instance.InputActions;

        if (_inputAction == null)
        {
            Debug.LogError("[FollowCamera] InputActions is null");
            return;
        }

        _inputAction.Spectator.NextPlayer.performed += SwitchToNext;
        _inputAction.Spectator.PrevPlayer.performed += SwitchToPrevious;

        // 처음엔 로컬 플레이어를 따라가도록 보장
        var local = PlayerInfoManager.Instance?.LocalPlayer;
        if (local != null)
        {
            SetTarget(local.transform, true);
            TrackLast(local.transform);
        }
    }

    private void OnDestroy()
    {
        if (_inputAction != null)
        {
            _inputAction.Spectator.NextPlayer.performed -= SwitchToNext;
            _inputAction.Spectator.PrevPlayer.performed -= SwitchToPrevious;
            _inputAction.Spectator.Disable();
        }
    }

    // 관전 모드 ON
    public void EnableSpectator()
    {
        if (_inputAction != null)
        {
            _inputAction.Player.Disable();
            _inputAction.Global.Disable();
            _inputAction.Spectator.Enable();
        }

        RebuildTargets();               // 최신 플레이어로 목록 갱신
        MoveIndexToFirstValid();        // 시작 인덱스 정렬
        SwitchToTarget(_currentIndex);  // 즉시 전환
    }

    // 관전 모드 OFF (로컬로 복귀)
    public void DisableSpectator()
    {
        if (_inputAction != null)
        {
            _inputAction.Player.Enable();
            _inputAction.Global.Enable();
            _inputAction.Spectator.Disable();
        }

        var local = PlayerInfoManager.Instance?.LocalPlayer;
        if (local != null)
        {
            SetTarget(local.transform, true); // 즉시 스냅 복귀
            TrackLast(local.transform);
        }
        _currentIndex = -1;
        _targets.Clear();
        _lastPos.Clear();

        RaiseTargetChanged(cmCam?.Target.TrackingTarget);
    }

    /// <summary>현재 실시간 상황으로 관전 후보 재구성</summary>
    public void RebuildTargets()
    {
        _targets.Clear();

        var mgr = PlayerInfoManager.Instance;
        if (mgr == null) return;

        // 로컬 제외 + 살아있는 대상만
        foreach (var p in PlayerInfoManager.PlayerControllers.Values)
        {
            if (p == null || p == mgr.LocalPlayer) continue;
            if (p.PlayerFSM != null && !p.PlayerFSM.IsDead)
            {
                _targets.Add(p);
                TrackLast(p.transform);
            }
        }
    }

    private void TrackLast(Transform t)
    {
        if (!t) return;
        _lastPos[t] = t.position;
    }

    public void SetTarget(Transform target, bool isInstant = false)
    {
        if (!cmCam) return;

        var prevTarget = cmCam.Target.TrackingTarget;
        Vector3 prevPos = prevTarget ? prevTarget.position : target ? target.position : Vector3.zero;

        cmCam.Target.TrackingTarget = target;

        if (isInstant && target != null)
        {
            var delta = target.position - prevPos;
            if (prevTarget != null)
            {
                cmCam.OnTargetObjectWarped(target, delta);
            }
            else if (delta.sqrMagnitude > 0.0001f)
            {
                cmCam.OnTargetObjectWarped(target, delta);
            }
        }

        if (prevTarget != target)
            RaiseTargetChanged(target);
    }

    private void SwitchToNext(InputAction.CallbackContext _) => SwitchToTarget(_currentIndex + 1);
    private void SwitchToPrevious(InputAction.CallbackContext _) => SwitchToTarget(_currentIndex - 1);
    
    public void SpectateNext() => SwitchToTarget(_currentIndex + 1);
    public void SpectatePrev() => SwitchToTarget(_currentIndex - 1);

    private void SwitchToTarget(int index)
    {
        if (_targets.Count == 0)
        {
            // 대상이 비었으면 즉시 재구성 시도
            RebuildTargets();
            if (_targets.Count == 0) return;
        }

        // 죽은 대상/사라진 대상 제거
        PruneInvalidTargets();
        if (_targets.Count == 0) return;

        if (index < 0) index = _targets.Count - 1;
        else if (index >= _targets.Count) index = 0;

        _currentIndex = index;

        var p = _targets[_currentIndex];
        if (p != null)
        {
            SetTarget(p.transform, true);
            TrackLast(p.transform);
        }
    }

    private void MoveIndexToFirstValid()
    {
        PruneInvalidTargets();
        _currentIndex = _targets.Count > 0 ? 0 : -1;
    }

    private void PruneInvalidTargets()
    {
        // null/죽음 제거
        for (int i = _targets.Count - 1; i >= 0; --i)
        {
            var p = _targets[i];
            if (p == null || p.PlayerFSM == null || p.PlayerFSM.IsDead)
                _targets.RemoveAt(i);
        }
    }

    private void LateUpdate()
    {
        // 현재 관전 대상 텔레포트 감지 → 워프로 스냅
        if (!cmCam) return;
        var t = cmCam.Target.TrackingTarget;
        if (!t) return;

        if (_lastPos.TryGetValue(t, out var last))
        {
            var delta = t.position - last;
            if (delta.sqrMagnitude >= _teleportSqrThreshold)
                cmCam.OnTargetObjectWarped(t, delta);

            _lastPos[t] = t.position;
        }
        else
        {
            _lastPos[t] = t.position;
        }
    }

    private void RaiseTargetChanged(Transform trackingTarget)
    {
        Player player = null;
        if (trackingTarget != null)
        {
            trackingTarget.TryGetComponent(out player);
            if (player == null) player = trackingTarget.GetComponentInParent<Player>();
        }

        CurrentSpectatedPlayer = player;
        SpectateTargetChanged?.Invoke(trackingTarget, player);
    }
}
