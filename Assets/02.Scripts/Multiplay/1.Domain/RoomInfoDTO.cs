using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;

// 수현
[FirestoreData]
public class RoomInfoDTO
{
    [FirestoreDocumentId] public string RoomInfoID { get; set; }

    [FirestoreProperty] public string RoomName { get; set; }
    [FirestoreProperty] public List<int> KnownIngredientsList { get; set; }
    [FirestoreProperty] public List<int> KnownRecipesList { get; set; }

    // Firestore 직렬화를 위해 parameterless 생성자 필수
    public RoomInfoDTO()
    {
    }
    
    public RoomInfoDTO (RoomInfo roomInfo)
    {
        RoomName = roomInfo.RoomName;
        KnownIngredientsList = roomInfo.KnownIngredients.ToList();
        KnownRecipesList = roomInfo.KnownRecipes.ToList();
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(this);
    }
}
