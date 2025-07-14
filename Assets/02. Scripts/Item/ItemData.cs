public class ItemData
{
    // 아이템에 공통적인 데이터 담는 클래스
    public readonly int ID;
    public readonly string Name;
    public readonly string Description;
    public readonly int MaxQuantity;
    // Addressable Path
    public readonly string IconAddressablePath;
    public readonly string ObjectAddressablePath;
    // .. 등등 추가 예정

    public ItemData(int id, string name, string description)
    {
        // TODO: 유효성 검사
        ID = id;
        Name = name;
        Description = description;
    }
}