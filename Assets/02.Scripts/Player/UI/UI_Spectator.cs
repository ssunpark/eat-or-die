using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Spectator : MonoBehaviour
{
    [Header("Wiring (Inspector)")]
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _nicknameText;

    private FollowCamera _followCam;
    private Camera _cachedMainCamera;

    private void Awake()
    {
        WireButtons();
        TryResolveFollowCamera();
        RefreshNicknameLabel();
    }

    private void OnEnable()
    {
        if (_followCam == null)
            TryResolveFollowCamera();

        RefreshNicknameLabel();
    }

    private void OnDisable()
    {
        UnsubscribeFollowCam();
    }

    private void OnDestroy()
    {
        UnwireButtons();
        UnsubscribeFollowCam();
    }

    private void WireButtons()
    {
        if (_prevButton != null) _prevButton.onClick.AddListener(OnClickPrev);
        if (_nextButton != null) _nextButton.onClick.AddListener(OnClickNext);
    }

    private void UnwireButtons()
    {
        if (_prevButton != null) _prevButton.onClick.RemoveListener(OnClickPrev);
        if (_nextButton != null) _nextButton.onClick.RemoveListener(OnClickNext);
    }

    private void OnClickPrev()
    {
        if (_followCam == null) return;
        _followCam.SpectatePrev();

        RefreshNicknameLabel();
    }

    private void OnClickNext()
    {
        if (_followCam == null) return;
        _followCam.SpectateNext();
        RefreshNicknameLabel();
    }

    private void TryResolveFollowCamera()
    {
        if (_followCam != null) return;
        if (_cachedMainCamera == null)
            _cachedMainCamera = Camera.main;
        var cam = _cachedMainCamera;
        if (cam != null) _followCam = cam.GetComponent<FollowCamera>();

        // 찾았으면 이벤트 구독
        if (_followCam != null)
        {
            _followCam.SpectateTargetChanged += OnSpectateTargetChanged;
        }
    }

    private void UnsubscribeFollowCam()
    {
        if (_followCam != null)
        {
            _followCam.SpectateTargetChanged -= OnSpectateTargetChanged;
        }
    }

    private void OnSpectateTargetChanged(Transform trackingTarget, Player player)
    {
        RefreshNicknameLabel(player);
    }

    private void RefreshNicknameLabel(Player playerOverride = null)
    {
        if (_nicknameText == null) return;

        Player player = playerOverride ?? _followCam?.CurrentSpectatedPlayer;
        if (player == null)
        {
            _nicknameText.text = "—";
            return;
        }

        var name = PlayerInfoManager.Instance?.GetNickname(player.Object.InputAuthority);
        _nicknameText.text = string.IsNullOrEmpty(name) ? "Unknown" : name;
    }
}
