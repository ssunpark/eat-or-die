using System;
using Fusion;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : NetworkBehaviour, IPickable
{
    [Networked] public int ItemID { get; set; }
    [Networked] public int Quantity { get; set; }
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public override void Spawned()
    {
        var icon = ItemManager.Instance.GetItem(ItemID).ItemData.Icon;
        ApplyVisual(icon);
    }

    public ItemStack Pick()
    {
        // 이 아이템을 주운 경우
        Debug.Log($"주운 아이템: ID - {ItemID}, {Quantity}개");
        return new ItemStack(ItemID, ItemManager.Instance.GetItem(ItemID).ItemData.MaxQuantity, Quantity);
    }

    // 외형 적용
    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}