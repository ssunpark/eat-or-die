using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;

[FirestoreData]
public class RoomInfoDTO
{
    [FirestoreDocumentId] public string RoomInfoID { get; set; }

    [FirestoreProperty] public string RoomName { get; set; }
    [FirestoreProperty] public List<int> KnownIngredientsList { get; set; }
    [FirestoreProperty] public List<int> KnownRecipesList { get; set; }

    
    public RoomInfoDTO()
    {
    }
    
    public RoomInfoDTO (RoomInfo roomInfo)
    {
        if (!string.IsNullOrEmpty(roomInfo.ID))
        {
            RoomInfoID = roomInfo.ID;
        }
        RoomName = roomInfo.RoomName;
        KnownIngredientsList = roomInfo.KnownIngredients.ToList();
        KnownRecipesList = roomInfo.KnownRecipes.ToList();
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(this);
    }
}

// 이 클래스는 오직 JsonUtility를 통한 네트워크 직렬화에만 사용됩니다.
[Serializable]
public class RoomInfoNetworkDTO
{
    public string ID;
    public string RoomName;
    public List<int> KnownIngredientsList;
    public List<int> KnownRecipesList;
}