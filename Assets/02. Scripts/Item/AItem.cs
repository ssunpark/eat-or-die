// 외부 클래스에서 이벤트를 발생 시키기 위한 추상 클래스
public abstract class AItem
{
    public readonly ItemData ItemData;

    protected AItem(ItemData itemData)
    {
        ItemData = itemData;
    }
    
    // 동작에 따른 분리
    // 다른 동작이 추가된다면 추가
    public abstract void OnSlotEvent();
    public abstract void OnUseEvent();
}