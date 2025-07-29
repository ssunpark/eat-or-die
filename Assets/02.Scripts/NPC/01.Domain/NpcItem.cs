using CsvHelper.Configuration.Attributes;

public class NpcItem
{
    [Name("NPC ID")]
    public int NpcID { get; set; }
    
    [Name("아이템 ID")]
    public int ItemID { get; set; }
    
    [Name("아이템 명")]
    public string ItemName { get; set; }
    
    [Name("가격")]
    public int Price { get; set; }
    
    [Name("재고수량")]
    public int StockQuantity { get; set; } // 무한개는 -1로 처리
    
    public bool IsInfinite => StockQuantity == -1;
}