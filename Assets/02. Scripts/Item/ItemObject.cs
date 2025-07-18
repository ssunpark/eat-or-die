using System;
using Fusion;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : NetworkBehaviour, IPickable
{
    [Networked]
    public int ItemID { get; set; }
    [Networked]
    public int Quantity { get; set; }
    private NetworkId _targetID;
    private bool _hasOwner;
    public bool HasOwner { get => _hasOwner; set => _hasOwner = value; }

    [SerializeField]
    private float _absorbSpeed = 10f;
    [SerializeField]
    private float _absorbThreshold = 0.1f;

    // 흡수 대상
    private Transform _target;

    private Collider _collider;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider>();
    }

    public override void Spawned()
    {
        Debug.Log("Spawned");
        // Debug.Log($"[Spawned] {name}, Object.IsValid: {Object.IsValid}, Behaviour Count: {Object.Behaviours?.Length}");
        Debug.Log($"[Spawned] {name}, Active: {gameObject.activeInHierarchy}, Enabled: {enabled}");
        var icon = ItemManager.Instance.GetItem(ItemID).ItemData.Icon;
        ApplyVisual(icon);
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer && _target != null)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position, _absorbSpeed * Runner.DeltaTime);

            if (Vector3.Distance(transform.position, _target.position) < _absorbThreshold)
            {
                // 인벤에 등록 요청
                RPC_AddInventory(_target.GetComponent<NetworkObject>().InputAuthority);
                _target = null;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AddInventory(PlayerRef targetPlayerRef)
    {
        if (Runner.LocalPlayer != targetPlayerRef)
        {
            return;
        }
        
        ItemStack itemStack = new ItemStack(ItemID,
            ItemManager.Instance.GetItem(ItemID).ItemData.MaxQuantity, Quantity);
        InventoryManager.Instance.PickItemFromGround(itemStack);
        
        RPC_RequestDespawn(Object.Id);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RequestDespawn(NetworkId targetId, RpcInfo info = default)
    {
        Debug.Log($"[Server] Despawn 요청 from: {info.Source}");

        var obj = Runner.FindObject(targetId);
        if (obj != null && obj.HasStateAuthority)
        {
            Runner.Despawn(obj);
            Debug.Log("[Server] Despawn 성공");
        }
        else
        {
            Debug.LogWarning("[Server] Despawn 실패 - 대상 없음 or 권한 없음");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Pick(NetworkId targetNetworkId)
    {
        _hasOwner = true;

        // 이 아이템을 주운 경우 줍기 비활성화
        _collider.enabled = false;

        // 아이템 흡수 연출 타겟 설정
        _targetID = targetNetworkId;
        _target = Runner.FindObject(targetNetworkId).gameObject.transform;

        Debug.Log($"주운 아이템: ID - {ItemID}, {Quantity}개");
    }

    // 외형 적용
    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}