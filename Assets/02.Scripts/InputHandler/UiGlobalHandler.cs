using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class UiGlobalHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AUI_PopupBase _traitsPopup;   // Traits 팝업 루트(= DefaultPopup 붙은 오브젝트)
    [SerializeField] private TraitsPanel _traitsPanel;      // TraitsPanel (동일 팝업 아래에 있음)
    [SerializeField] private StatsPanel _statsPanel;        // StatsPanel (동일 팝업 아래에 있음, TraitsPanel과는 다른 오브젝트)
    [SerializeField] private AUI_PopupBase _partyPopup;     // 파티 HP 창 루트(= DefaultPopup/AnimatePopup 포함)
    [SerializeField] private UI_HUDPartyHP _partyHud;       
    [SerializeField] private UI_SkillTree _skillTree;
    [SerializeField] private ProfilePanel _profilePanel;
    [SerializeField] private UI_Inventory _inventoryPanel;
    [SerializeField] private List<AUI_PopupBase> _inputBlockPopups;
    private AUI_PopupBase _statsPopup;

    // 액션/맵
    private PlayerInputActions _actions;
    private InputAction _toggleInventory;
    private InputAction _toggleTraits;
    private InputAction _toggleParty;

    // 맵 전환용
    private InputActionMap _mapPlayer;
    private InputActionMap _mapUI;
    private InputActionMap _mapGlobal;

    private int _modalCount; // UI 맵 켜둘지 결정 (Traits 같은 모달 팝업 수)
    private bool _initialized = false;

    private void Start()
    {
        _actions = InputReader.Instance.InputActions;
        _mapPlayer = _actions.Player;
        _mapUI = _actions.UI;
        _mapGlobal = _actions.Global;

        _toggleInventory = _actions.Global.ToggleInventory;
        _toggleTraits = _actions.Global.ToggleTraits;
        _toggleParty = _actions.Global.TogglePartyHud;
        _statsPopup = _statsPanel.GetComponent<AUI_PopupBase>();

        _mapGlobal.Enable();

        _toggleTraits.performed += OnToggleTraits;
        _toggleParty.performed += OnToggleParty;
        _toggleInventory.performed += OnToggleInventory;

        if (_traitsPopup != null)
        {
            _traitsPopup.Opened += OnModalOpened;
            _traitsPopup.Closed += OnModalClosed;
        }

        if(_inputBlockPopups.Count>0)
        {
            foreach (var popup in _inputBlockPopups)
            {
                popup.Opened += OnModalOpened;
                popup.Closed += OnModalClosed;
            }
        }

        // 시작 시 UI 맵은 꺼두고, 플레이어 맵만 활성
        EnsureMaps(modalOpen: false);
        _initialized = true;
    }

    private void OnDisable()
    {
        if(_toggleTraits!=null)
        _toggleTraits.performed -= OnToggleTraits;
        if(_toggleParty!=null)
            _toggleParty.performed -= OnToggleParty;
        if(_toggleInventory!=null)
            _toggleInventory.performed -= OnToggleInventory;
        if (_mapGlobal != null)
            _mapGlobal.Disable();
    }

    // ===== Traits =====
    private void OnToggleTraits(InputAction.CallbackContext _)
    {
        if (_traitsPopup == null) return;

        bool open = !_traitsPopup.gameObject.activeSelf;
        if (open)
        {
            BindPlayer();

            _traitsPopup.Open();
        }
        else
        {
            UnbindPlayer();
            StartCoroutine(CloseTraitCoroutine());
        }
    }

    private IEnumerator CloseTraitCoroutine()
    {
        if (_statsPopup.gameObject.activeInHierarchy)
        {
            _statsPopup.Close();
            while (_statsPopup.gameObject.activeInHierarchy)
                yield return null;
        }
        _traitsPopup.Close();
    }

    private void UnbindPlayer()
    {
        _statsPanel?.Unbind();

        if (_traitsPanel != null)
            _traitsPanel.Unbind();

        if (_skillTree != null)
            _skillTree.Unbind();
    }

    bool _isProfileBound = false;

    private void BindPlayer()
    {
        var local = FindLocalPlayer();
        if (local != null && _traitsPanel != null && _statsPanel != null && _skillTree != null)
        {
            _traitsPanel.BindLocal(local);
            _statsPanel.BindLocal(local);
            _skillTree.BindLocal(local);
        }
        if (_isProfileBound == false)
        {
            if(local.TryGetComponent(out PlayerCustomizeHandler pch))
                _isProfileBound = _profilePanel.BindLocal(pch);
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

    // ===== Inventory =====
    public void OnToggleInventory(InputAction.CallbackContext _)
    {
        if (_inventoryPanel == null) return;
        bool open = !_inventoryPanel.gameObject.activeSelf;
        if (open)
        {
            _inventoryPanel.Open();
        }
        else
        {
            _inventoryPanel.Close();
        }
    }


    // ===== Helpers =====
    private Player FindLocalPlayer()
    {
        return Room.Instance.LocalPlayer?.GetComponent<Player>();
    }

    private void EnsureMaps(bool modalOpen)
    {
        if (!_initialized) return;
        if (modalOpen)
        {
            _mapPlayer.Disable();

        }
        else
        {
            _mapPlayer.Enable();
        }
    }

    private void OnModalOpened(AUI_PopupBase popupBase)
    {
        _modalCount++;
        EnsureMaps(true);
    }

    private void OnModalClosed(AUI_PopupBase popupBase)
    {
        if (!_initialized) return;
        if (popupBase == _traitsPopup)
        {
            UnbindPlayer();
        }
        _modalCount = Mathf.Max(0, _modalCount - 1);
        EnsureMaps(_modalCount > 0);
    }

    private void LateUpdate()
    {
        if (!_initialized) return;
        if (_modalCount > 0 && AllModalPopupIsInactive)
        {
            _modalCount = 0; // 모든 모달 팝업이 닫혔으면 카운트 초기화
            EnsureMaps(false); // 플레이어 맵 활성화
        }
    }

    private bool AllModalPopupIsInactive
    {
        get
        {
            if (_traitsPopup != null && _traitsPopup.gameObject.activeSelf) return false;
            if (_statsPopup != null && _statsPopup.gameObject.activeSelf) return false;
            if (_partyPopup != null && _partyPopup.gameObject.activeSelf) return false;
            foreach (var popup in _inputBlockPopups)
            {
                if (popup.gameObject.activeSelf) return false;
            }
            return true;
        }
    }
}
