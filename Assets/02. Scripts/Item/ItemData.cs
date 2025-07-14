public class ItemData
{
    // 아이템에 공통적인 데이터 담는 클래스
    public readonly string ID;
    public readonly EItemType ItemType;
    public readonly string Name;
    public readonly string Description;
    // Addressable Path
    public readonly string IconAddressablePath;
    public readonly string ObjectAddressablePath;
    // .. 등등 추가 예정

    public ItemData(string id, string name, string description)
    {
        // TODO: 유효성 검사
        ID = id;
        Name = name;
        Description = description;
    }
}