using UnityEngine;
using UnityEngine.UI;

public class UI_Logout : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;

    [Header("Login")]
    public FadeController NeedLoginNotification;
    private bool _isLogin => AuthenticationManager.Instance.User != null;

    [Header("특정 버튼으로 팝업 열기")]
    [SerializeField] private Button _logoutButton;
    [SerializeField] private AUI_PopupBase _popupLogoutCheck;

    protected override void Awake()
    {

        if (_logoutButton != null)
            _logoutButton.onClick.AddListener(OnOpenPopupButtonClicked);
    }

    private void OnDestroy()
    {

        if (_logoutButton != null)
            _logoutButton.onClick.RemoveListener(OnOpenPopupButtonClicked);
    }

    public override void Open()
    {
        if (!_isLogin)
        {
            ShowNeedLoginNotification();
            return;
        }

        base.Open();
    }

    private void OnOpenPopupButtonClicked()
    {
        TryOpenPopup(_popupLogoutCheck);
    }

    private void TryOpenPopup(AUI_PopupBase popup)
    {
        if (!_isLogin)
        {
            ShowNeedLoginNotification();
            return;
        }

        if (popup == null)
        {
            Debug.LogWarning("[UI_RoomListPopup] Target popup is not assigned.");
            return;
        }

        popup.Open();

        // 캐릭터 선택 팝업이면 갱신까지
        if (popup is UI_CharacterSelect cs)
            cs.Refresh();
    }

    private void ShowNeedLoginNotification()
    {
        if (NeedLoginNotification == null)
        {
            Debug.LogError("NeedLoginNotification is null");
            return;
        }
        NeedLoginNotification.FadeIn();
        NeedLoginNotification.FadeOutAfterDelay();
    }
}
