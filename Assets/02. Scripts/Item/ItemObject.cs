using System;
using Fusion;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : NetworkBehaviour, IPickable
{
    [Networked] public int ItemID { get; set; }
    [Networked] public int Quantity { get; set; }
    [Networked] public bool HasOwner { get; set; }

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
        var icon = ItemManager.Instance.GetItem(ItemID).ItemData.Icon;
        ApplyVisual(icon);
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsProxy && _target != null)
        {
            // 서버 또는 InputAuthority 있는 클라이언트만 위치 갱신
            transform.position = Vector3.Lerp(transform.position, _target.position, _absorbSpeed * Runner.DeltaTime);

            if (Vector3.Distance(transform.position, _target.position) < _absorbThreshold)
            {
                _target = null;
                // 인벤에 등록
                ItemStack itemStack = new ItemStack(ItemID, ItemManager.Instance.GetItem(ItemID).ItemData.MaxQuantity, Quantity);
                InventoryManager.Instance.PickItemFromGround(itemStack);
                Runner.Despawn(Object);
            }
        }
    }

    public void Pick(GameObject target)
    {
        // 이 아이템을 주운 경우 줍기 비활성화
        _collider.enabled = false;
        
        // 아이템 흡수 연출 타겟 설정
        _target = target.transform;
        
        Debug.Log($"주운 아이템: ID - {ItemID}, {Quantity}개");
    }

    // 외형 적용
    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}