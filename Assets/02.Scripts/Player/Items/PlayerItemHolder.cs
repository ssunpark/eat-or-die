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
    public int HoldItemID { get; private set; }
    public override void Spawned()
    {
        if (HoldItemID > 0)
        {
            Debug.Log($"[PlayerItemHolder] Spawned - Item with ID {HoldItemID} is being held. Synchronizing state.");
            _HoldItemLogic(HoldItemID);
        }
    }
    private void _HoldItemLogic(int itemId)
    {
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }

        if (_heldItemObject != null)
        {
            var heldItem = ItemManager.Instance.GetItem(HoldItemID);
            heldItem?.UnHoldItem(gameObject, _heldItemObject);
            _heldItemObject = null;
        }

        ItemProfile changedHoldItem = ItemManager.Instance.GetItem(itemId);
        if (changedHoldItem == null)
        {
            Debug.LogError($"[PlayerItemHolder] 아이템 정보가 없습니다. ID: {itemId}");
            return;
        }

        AttackType = changedHoldItem.ItemDefinition.AttackType;
        ProjectileKey = changedHoldItem.ItemDefinition.ProjectileKey;

        changedHoldItem.HoldItem(gameObject);
        _heldItemObject = changedHoldItem.GetHoldItemObject();
        _heldItemObject.transform.SetParent(_handTransform);

        _heldItemObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        _heldItemObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        _heldItemObject.transform.localScale = Vector3.one;


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
           _playerFSM.StateMachine.ActiveStateId == (int)EPlayerState.Cooking)
        {
            return true;
        }
        return false;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_RequestUnholdItem()
    {
        if (_player == null)
        {
            _player = GetComponent<Player>();
        }
        var heldItem = ItemManager.Instance.GetItem(HoldItemID);
        heldItem?.UnHoldItem(gameObject, _heldItemObject);
        _heldItemObject = null;
        HoldItemID = -1;

        AttackType = EAttackType.MeleeWeapon;
        ProjectileKey = "DefaultProjectile";
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_RequestHoldItem(int itemId)
    {
        _HoldItemLogic(itemId);
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