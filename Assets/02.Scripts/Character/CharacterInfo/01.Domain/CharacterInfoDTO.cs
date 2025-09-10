using Firebase.Firestore;

[FirestoreData]
public class CharacterInfoDTO
{
    [FirestoreDocumentId] public string Id { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public Timestamp CreatedAt { get; set; }
    [FirestoreProperty] public Timestamp LastLoginAt { get; set; }

    public CharacterInfoDTO()
    {
    }
    
    public CharacterInfoDTO(CharacterInfo characterInfo)
    {
        Id = characterInfo.Id;
        Name = characterInfo.Name;
        CreatedAt = characterInfo.CreatedAt;
        LastLoginAt = characterInfo.LastLoginAt;
    }
}
