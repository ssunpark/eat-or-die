using System;
using Fusion;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : MonoBehaviour, IPickable
{
    private ItemStack _itemStack;
    public ItemStack ItemStack => _itemStack;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Init(ItemStack itemStack)
    {
        _itemStack = itemStack;
        // Stack에 있는 ID를 통해 외형 정보 가져오기
        var icon = ItemManager.Instance.GetItem(itemStack.ID).ItemData.Icon;
        ApplyVisual(icon);
    }

    public ItemStack Pick()
    {
        // 이 아이템을 주운 경우
        Debug.Log($"주운 아이템: ID - {_itemStack.ID}, {_itemStack.Quantity}개");
        return _itemStack;
    }

    // 외형 적용
    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}