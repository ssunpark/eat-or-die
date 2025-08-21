using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System.Collections;

public class PlayerItemHolder: NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _handTransform;
    private PlayerFSM _playerFSM;
    public EAttackType AttackType = EAttackType.MeleeWeapon;
    public ItemInstance HeldItemInstance { get; private set; }
    private GameObject _heldItemObject;
    public GameObject HeldItemObject => _heldItemObject;
    public string InteractionTag;

    private Player _player;

    public string ProjectileKey;
    [System.Serializable]
    public class AnimatorOverrideEntry
    {
        public string key;
        public AnimatorOverrideController controller;
    }
    [SerializeField] private List<AnimatorOverrideEntry> _overrideList;

    private Dictionary<string,AnimatorOverrideController> _animatorOverrideMap = new();
    [Networked, OnChangedRender(nameof(OnHoldItemIDChanged))]
    public int HoldItemID { get; set; }
    public override void Spawned()
    {
        if (HoldItemID > 0) _HoldItemLogic(HoldItemID);
        else _UnholdItemLogic();
    }

    private void OnHoldItemIDChanged()
    {
        if (HoldItemID > 0) _HoldItemLogic(HoldItemID);
        else _UnholdItemLogic();
    }

    
    private void _HoldItemLogic(int itemId)
    {
        if (_player == null)
            _player = GetComponent<Player>();

        // 이전 장착 해제 (이전값은 _currentHeldId로)
        if (_heldItemObject != null)
        {
            var prev = _currentHeldId > 0 ? ItemManager.Instance.GetItem(_currentHeldId) : null;
            prev?.UnHoldItem(gameObject, _heldItemObject);
            _heldItemObject = null;
        }

        var changed = ItemManager.Instance.GetItem(itemId);
        if (changed == null)
        {
            Debug.LogError($"[PlayerItemHolder] 아이템 정보가 없습니다. ID: {itemId}");
            return;
        }

        AttackType = changed.ItemDefinition.AttackType;
        ProjectileKey = changed.ItemDefinition.ProjectileKey;

        changed.HoldItem(gameObject);
        _heldItemObject = changed.GetHoldItemObject();
        _heldItemObject.transform.SetParent(_handTransform, false);
        _heldItemObject.transform.localPosition = Vector3.zero;
        _heldItemObject.transform.localRotation = Quaternion.identity;
        _heldItemObject.transform.localScale = Vector3.one;

        _currentHeldId = itemId;
    }
    private int _currentHeldId = -1;
    private void _UnholdItemLogic()
    {
        if (_heldItemObject != null)
        {
            var prev = _currentHeldId > 0 ? ItemManager.Instance.GetItem(_currentHeldId) : null;
            prev?.UnHoldItem(gameObject, _heldItemObject);
            _heldItemObject = null;
        }
        HeldItemInstance = null;
        _currentHeldId = -1;

        AttackType = EAttackType.MeleeWeapon;
        ProjectileKey = "DefaultProjectile";
    }
    private void Awake()
    {
        // 얘도 나중에 어드레서블 로드 후 key 기반 로딩
        _animatorOverrideMap = new Dictionary<string, AnimatorOverrideController>();
        foreach (var entry in _overrideList)
        {
            _animatorOverrideMap[entry.key] = entry.controller;
        }
        _playerFSM = GetComponent<PlayerFSM>();
    }

    public void UseItem(GameObject target)
    {
        QuickSlotManager.Instance.UseItem(target, RPC_RequestUnholdItem);
    }
    

    public void SetHoldItem(ItemInstance itemInstance)
    {
        if (_setHoldItemCoroutine != null)
        {
            StopCoroutine(_setHoldItemCoroutine);
            _setHoldItemCoroutine = null;
        }
        StartCoroutine(SetHoldItemCoroutine(itemInstance));
    }

    private IEnumerator SetHoldItemCoroutine(ItemInstance itemInstance)
    {
        yield return new WaitUntil(CanChangeItem);

        yield return new WaitForSeconds(0.05f);
        HeldItemInstance = itemInstance;

        if (HasInputAuthority)
        {
            if (itemInstance == null)
            {
                RPC_RequestUnholdItem();
            }
            else
                RPC_RequestHoldItem(itemInstance.ID);
        }
    }

    private bool CanChangeItem()
    {
        if(_playerFSM == null || _playerFSM.StateMachine == null)
        {
            return false;
        }
        if(_playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Idle||
            _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Move ||
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Hit ||
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Recover ||
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Cooking ||
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.CookSuccess ||
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Run
           )
        {
            return true;
        }
        return false;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestUnholdItem()
    {
        if (!HasStateAuthority) return;
        HoldItemID = -1;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_RequestHoldItem(int itemId)
    {
        if (!HasStateAuthority) return;
        HoldItemID = itemId;
    }

    Coroutine _setHoldItemCoroutine;
    public void ApplyAnimatorOverride(string key)
    {
        if (_animatorOverrideMap.TryGetValue(key, out var controller))
        {
            _animator.runtimeAnimatorController = controller;
        }
        else
        {
            if (!_animatorOverrideMap.ContainsKey("Unarmed"))
                Debug.LogWarning("Unarmed 애니메이터 오버라이드가 설정되지 않았습니다.");
            ApplyAnimatorOverride("Unarmed");
        }
        _player.CacheAnimationLengths();
    }
}