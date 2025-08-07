using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerItemHolder: NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _handTransform;
    private PlayerFSM _playerController;
    public EAttackType AttackType = EAttackType.MeleeWeapon;
    public Item HeldItem { get; private set; }
    private GameObject _heldItemObject;
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

    private void Awake()
    {
        // 얘도 나중에 어드레서블 로드 후 key 기반 로딩
        _animatorOverrideMap = new Dictionary<string, AnimatorOverrideController>();
        foreach (var entry in _overrideList)
        {
            _animatorOverrideMap[entry.key] = entry.controller;
        }
        _playerController = GetComponent<PlayerFSM>();
    }

    public void UseItem(GameObject target)
    {
        Debug.Log($"[PlayerItemHolder] UseItem Called. Target: {target.name}");
        QuickSlotManager.Instance.UseItem(target, RPC_RequestUnholdItem);
    }
    

    public void SetHoldItem(Item item)
    {
        Debug.Log($"[PlayerItemHolder] SetHoldItem Called.");

        HeldItem = item;

        if (HasInputAuthority)
        {
            if(item == null)
            {
                RPC_RequestUnholdItem();
            }
            else
                RPC_RequestHoldItem(item.ID);
        }
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
        _player.CacheAnimationLengths();

        AttackType = EAttackType.MeleeWeapon;
        ProjectileKey = "DefaultProjectile";
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_RequestHoldItem(int itemId)
    {
        if(_player == null)
        {
            _player = GetComponent<Player>();
        }
        if (HoldItemID > 0)
        {
            var heldItem = ItemManager.Instance.GetItem(HoldItemID);
            heldItem?.UnHoldItem(gameObject, _heldItemObject);
        }
        Debug.Log($"[PlayerItemHolder] RPC_RequestHoldItem Called. ID: {itemId}");
        HoldItemID = itemId;
        AItemInfo changedHoldItem = ItemManager.Instance.GetItem(itemId);
        if(changedHoldItem == null)
        {
            Debug.LogError($"[PlayerItemHolder] 아이템 정보가 없습니다. ID: {itemId}");
            return;
        }
        
        AttackType = changedHoldItem.ItemData.AttackType;
        ProjectileKey = changedHoldItem.ItemData.ProjectileKey;

        changedHoldItem.HoldItem(gameObject);
        _heldItemObject = changedHoldItem.GetHoldItemObject();
        _heldItemObject.transform.SetParent(_handTransform);

        // 손 위치와 회전 설정
        _heldItemObject.transform.localPosition = new Vector3(0.07f, 0.14f, -0.02f);
        _heldItemObject.transform.localRotation = Quaternion.Euler(-180f, 0f, 0f);
        _heldItemObject.transform.localScale = Vector3.one;

        _player.CacheAnimationLengths();
    }

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
    }
}