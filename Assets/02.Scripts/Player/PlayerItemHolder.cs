using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class PlayerItemHolder: NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    private PlayerController _playerController;

    [System.Serializable]
    public class AnimatorOverrideEntry
    {
        public string key;
        public AnimatorOverrideController controller;
    }
    [SerializeField] private List<AnimatorOverrideEntry> _overrideList;

    private Dictionary<string,AnimatorOverrideController> _animatorOverrideMap = new();
    [Networked, OnChangedRender(nameof(OnChangedHoldItem))] public int HoldItemID { get; set; }

    private void Awake()
    {
        // 얘도 나중에 어드레서블 로드 후 key 기반 로딩
        _animatorOverrideMap = new Dictionary<string, AnimatorOverrideController>();
        foreach (var entry in _overrideList)
        {
            _animatorOverrideMap[entry.key] = entry.controller;
        }
        _playerController = GetComponent<PlayerController>();
    }



    public void SetHoldItem(AItemInfo itemInfo)
    {
        Debug.Log($"[PlayerItemHolder] SetHoldItem Called.");
        GetComponent<PlayerInteractions>().OnEquipped(itemInfo);
        if (!_playerController.HasInputAuthority) return;
        RPC_SetHoldItemID(itemInfo?.ItemData.ID??0);
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetHoldItemID(int itemID)
    {
        HoldItemID = itemID;
        Debug.Log($"[PlayerItemHolder] HoldItemID changed to {HoldItemID}");
    }


    // 나중에 csv에 영어이름이 추가되면 ItemDatabase 필요 없고?
    // ApplyAnimatorOverride(영어 이름)으로 변경
    // 
    private void OnChangedHoldItem()
    {
        Debug.Log($"[PlayerItemHolder] OnChangedHoldItem Called.");
        if (ItemDatabase.TryGetWeaponType(HoldItemID, out var weaponType))
        {
            switch (weaponType)
            {
                case EWeaponType.Sword:
                    ApplyAnimatorOverride("Sword");
                    break;
                case EWeaponType.Axe:
                    ApplyAnimatorOverride("Axe");
                    break;
                case EWeaponType.Staff:
                    ApplyAnimatorOverride("Staff");
                    break;
            }
        }
        else if (ItemDatabase.TryGetUseAction(HoldItemID, out var useAction))
        {
            switch (useAction)
            {
                case EUseAction.Plant:
                    ApplyAnimatorOverride("Seed");
                    break;
                case EUseAction.Plow:
                    ApplyAnimatorOverride("Hoe");
                    break;
                case EUseAction.Water:
                    ApplyAnimatorOverride("WateringCan");
                    break;
            }
        }
        else
        {
            ApplyAnimatorOverride("Unarmed");
        }
    }
    private void ApplyAnimatorOverride(string key)
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