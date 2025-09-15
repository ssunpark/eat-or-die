using Firebase.Firestore;

[FirestoreData]
public class CharacterInfoDTO
{
    [FirestoreDocumentId] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public bool IsInit { get; set; }
    [FirestoreProperty] public int Class { get; set; }
    [FirestoreProperty] public Timestamp CreatedAt { get; set; }
    [FirestoreProperty] public Timestamp LastLoginAt { get; set; }
    [FirestoreProperty] public int Top { get; set; }
    [FirestoreProperty] public int Bottom { get; set; }
    [FirestoreProperty] public int Hair { get; set; }
    [FirestoreProperty] public int Eye { get; set; }
        

    public CharacterInfoDTO()
    {
    }
    
    public CharacterInfoDTO(CharacterInfo characterInfo)
    {
        Id = characterInfo.Id;
        Name = characterInfo.Name;
        IsInit = characterInfo.IsInit;
        Class = (int)characterInfo.Class;
        CreatedAt = characterInfo.CreatedAt;
        LastLoginAt = characterInfo.LastLoginAt;
        Top = characterInfo.Top;
        Bottom = characterInfo.Bottom;
        Hair = characterInfo.Hair;
        Eye = characterInfo.Eye;
    }
}
