using System;
using Firebase.Firestore;

// Firestore 문서 ↔ DTO 직렬화용
[Serializable, FirestoreData]
public class SkillDTO
{
    // Firestore 문서 ID를 담는 필드(예: "12")
    [FirestoreDocumentId]
    public string DocId { get; set; }

    // 실제 저장되는 필드(필요한 건 레벨뿐)
    [FirestoreProperty]
    public int Level { get; set; }

    // 기존 코드 호환용: int Id (DocId에서 파생)
    public int Id => int.TryParse(DocId, out var id) ? id : 0;

    public SkillDTO() { } // Firestore 역직렬화용

    public SkillDTO(int id, int level)
    {
        DocId = id.ToString();
        Level = level;
    }
}