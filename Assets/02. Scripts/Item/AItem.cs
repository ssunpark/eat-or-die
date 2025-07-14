public abstract class AItem
{
    // 아이템 효과 발동 이벤트
    // 동작에 따른 분리
    // 다른 동작이 추가된다면 추가
    public readonly ItemData ItemData;

    protected AItem(ItemData itemData)
    {
        ItemData = itemData;
    }
    
    public abstract void OnSlotEvent();
    public abstract void OnUseEvent();
}