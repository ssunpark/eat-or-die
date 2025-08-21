using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;

[FirestoreData]
public class RoomInfoDTO
{
    [FirestoreDocumentId] public string RoomInfoID { get; set; }
    [FirestoreProperty] public string RoomName { get; set; }
    [FirestoreProperty] public int MemberCount { get; set; }
    [FirestoreProperty] public List<string> MemberList { get; set; }
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
        // MemberCount = roomInfo.MemberCount;
        // MemberList = new List<string>(roomInfo.MemberList);
        KnownIngredientsList = roomInfo.KnownIngredients.ToList();
        KnownRecipesList = roomInfo.KnownRecipes.ToList();
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(this);
    }
}

[Serializable]
public class RoomInfoNetworkDTO
{
    public string ID;
    public string RoomName;
    public int MemberCount;
    public List<string> MemberList;
    public List<int> KnownIngredientsList;
    public List<int> KnownRecipesList;
}