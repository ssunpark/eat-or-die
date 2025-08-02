using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerItemHolder: NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    private PlayerFSM _playerController;
    public Item HeldItem { get; private set; }
    private GameObject _heldItemObject;
    public string InteractionTag;
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
        HeldItem?.Use(target);
    }
    

    public void SetHoldItem(Item item)
    {
        Debug.Log($"[PlayerItemHolder] SetHoldItem Called.");

        HeldItem = item;

        if (HasInputAuthority)
            RPC_RequestHoldItem(item.ID);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_RequestHoldItem(int itemId)
    {
        var heldItem = ItemManager.Instance.GetItem(itemId);
        heldItem?.UnHoldItem(gameObject, _heldItemObject);
        Debug.Log($"[PlayerItemHolder] RPC_RequestHoldItem Called. ID: {itemId}");
        HoldItemID = itemId;
        AItemInfo changedHoldItem = ItemManager.Instance.GetItem(itemId);
        if(changedHoldItem == null)
        {
            Debug.LogError($"[PlayerItemHolder] 아이템 정보가 없습니다. ID: {itemId}");
            return;
        }
        _heldItemObject = changedHoldItem.GetHoldItemObject();
        changedHoldItem.HoldItem(gameObject);
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