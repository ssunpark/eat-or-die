using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiGlobalHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AUI_PopupBase _traitsPopup;   // Traits 팝업 루트(= DefaultPopup 붙은 오브젝트)
    [SerializeField] private TraitsPanel _traitsPanel;      // TraitsPanel (동일 팝업 아래에 있음)
    [SerializeField] private StatsPanel _statsPanel;        // StatsPanel (동일 팝업 아래에 있음, TraitsPanel과는 다른 오브젝트)
    [SerializeField] private AUI_PopupBase _partyPopup;     // 파티 HP 창 루트(= DefaultPopup/AnimatePopup 포함)
    [SerializeField] private UI_HUDPartyHP _partyHud;       // 파티 HP 본문(선택)
    private AUI_PopupBase _statsPopup;

    // 액션/맵
    private PlayerInputActions _actions;
    private InputAction _toggleInventory; // 안 쓰면 제거해도 됨
    private InputAction _toggleTraits;
    private InputAction _toggleParty;

    // 맵 전환용
    private InputActionMap _mapPlayer;
    private InputActionMap _mapUI;
    private InputActionMap _mapGlobal;

    private int _modalCount; // UI 맵 켜둘지 결정 (Traits 같은 모달 팝업 수)

    private void Start()
    {
        _actions = InputReader.Instance.InputActions;
        _mapPlayer = _actions.Player;
        _mapUI = _actions.UI;
        _mapGlobal = _actions.Global;

        _toggleInventory = _actions.Global.ToggleInventory; // 필요 시 사용
        _toggleTraits = _actions.Global.ToggleTraits;
        _toggleParty = _actions.Global.TogglePartyHud;
        _statsPopup = _statsPanel.GetComponent<AUI_PopupBase>();

        _mapGlobal.Enable(); // 항상 켜둠

        _toggleTraits.performed += OnToggleTraits;
        _toggleParty.performed += OnToggleParty;

        if (_traitsPopup != null)
        {
            _traitsPopup.Opened += OnModalOpened;
            _traitsPopup.Closed += OnModalClosed;
        }

        // 시작 시 UI 맵은 꺼두고, 플레이어 맵만 활성
        EnsureMaps(modalOpen: false);
    }

    private void OnDisable()
    {
        _toggleTraits.performed -= OnToggleTraits;
        _toggleParty.performed -= OnToggleParty;
        _mapGlobal.Disable();
    }

    // ===== Traits =====
    private void OnToggleTraits(InputAction.CallbackContext _)
    {
        if (_traitsPopup == null) return;

        bool open = !_traitsPopup.gameObject.activeSelf;
        if (open)
        {
            var local = FindLocalPlayer();
            if (local != null && _traitsPanel != null && _statsPanel != null)
            {
                _traitsPanel.BindLocal(local);
                _statsPanel.BindLocal(local);
            }
                

            _traitsPopup.Open();
        }
        else
        {
            _statsPanel?.Unbind();
            if (_statsPopup.gameObject.activeInHierarchy)
            {
                _statsPopup.Close();
            }

            _traitsPopup.Close();
            if (_traitsPanel != null)
                _traitsPanel.Unbind();

        }
    }

    // ===== Party HUD =====
    private void OnToggleParty(InputAction.CallbackContext _)
    {
        if (_partyPopup == null) return;

        bool open = !_partyPopup.gameObject.activeSelf;
        if (open)
        {
            _partyPopup.Open();
        }
        else
        {
            _partyPopup.Close();
        }
    }

    // ===== Helpers =====
    private Player FindLocalPlayer()
    {
        return Room.Instance.LocalPlayer?.GetComponent<Player>();
    }

    private void EnsureMaps(bool modalOpen)
    {
        if (modalOpen)
        {
            _mapPlayer.Disable();
        }
        else
        {
            _mapPlayer.Enable();
        }
    }

    private void OnModalOpened(AUI_PopupBase _)
    {
        _modalCount++;
        EnsureMaps(true);
    }

    private void OnModalClosed(AUI_PopupBase _)
    {
        _modalCount = Mathf.Max(0, _modalCount - 1);
        EnsureMaps(_modalCount > 0);
    }

    private void LateUpdate()
    {
        if(_modalCount > 0 && AllModalPopupIsInactive)
        {
            _modalCount = 0; // 모든 모달 팝업이 닫혔으면 카운트 초기화
            EnsureMaps(false); // 플레이어 맵 활성화
        }
    }

    private bool AllModalPopupIsInactive
    {
        get
        {
            // Traits 팝업과 Stats 팝업이 모두 비활성화 상태인지 확인
            return (_traitsPopup == null || !_traitsPopup.gameObject.activeSelf) &&
                   (_statsPopup == null || !_statsPopup.gameObject.activeSelf);
        }
    }
}
