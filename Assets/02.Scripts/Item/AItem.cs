// 외부 클래스에서 이벤트를 발생 시키기 위한 추상 클래스
public abstract class AItem
{
    public readonly ItemData ItemData;

    protected AItem(ItemData itemData)
    {
        ItemData = itemData;
    }
}