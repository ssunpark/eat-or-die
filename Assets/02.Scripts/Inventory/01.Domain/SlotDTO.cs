using Firebase.Firestore;

[FirestoreData]
public class SlotDTO
{
    [FirestoreDocumentId] public string SlotId { get; set; }
    [FirestoreProperty] public int ItemId { get; set; }
    [FirestoreProperty] public int Quantity { get; set; }
    [FirestoreProperty] public float Durability { get; set; }
    [FirestoreProperty] public string ExtraInfo { get; set; }

    public SlotDTO()
    {
        
    }
    
    public SlotDTO(int slotId, ItemInstance itemInstance)
    {
        SlotId = slotId.ToString();
        
        if (itemInstance == null)
        {
            ItemId = 0;
            Quantity = 0;
            Durability = 0;
            ExtraInfo = "";
            return;
        }
        
        ItemId = itemInstance.ID;
        Quantity = itemInstance.Quantity;
        Durability = itemInstance.Durability;
        ExtraInfo = itemInstance.ExtraInfo;
    }
}