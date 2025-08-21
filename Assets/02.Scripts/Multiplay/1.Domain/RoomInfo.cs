using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Firestore;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    [Header("방 명세")]
    public string ID;
    public string RoomName;
    public Timestamp CreatedAt;
    // public int MemberCount;
    // public List<string> MemberList;
    
    [Header("요리 시스템")]
    public IReadOnlyCollection<int> KnownIngredients => _knownIngredients;
    public IReadOnlyCollection<int> KnownRecipes => _knownRecipes;

    [SerializeField] private HashSet<int> _knownIngredients;
    [SerializeField] private HashSet<int> _knownRecipes;

    public RoomInfo(string roomName) //, int memberCount, List<string> memberList
    {
        RoomName = roomName;
        CreatedAt = Timestamp.GetCurrentTimestamp();
        // MemberCount = memberCount;
        // MemberList = memberList;
        _knownIngredients = new HashSet<int>();
        _knownRecipes = new HashSet<int>();
    }

    public RoomInfo(RoomInfoNetworkDTO roomInfoNetworkDTO)
    {
        ID = roomInfoNetworkDTO.ID;
        RoomName = roomInfoNetworkDTO.RoomName;
        // MemberCount = roomInfoNetworkDTO.MemberCount;
        // MemberList = roomInfoNetworkDTO.MemberList;
        _knownIngredients = roomInfoNetworkDTO.KnownIngredientsList.ToHashSet();
        _knownRecipes = roomInfoNetworkDTO.KnownRecipesList.ToHashSet();
    }

    public RoomInfo(RoomInfoDTO roomInfoDTO)
    {
        ID = roomInfoDTO.RoomInfoID;
        RoomName = roomInfoDTO.RoomName ?? "Unnamed Room";
        CreatedAt = roomInfoDTO.CreatedAt;

        // MemberCount = roomInfoDTO.MemberCount;
        // MemberList = roomInfoDTO.MemberList != null
        //     ? new List<string>(roomInfoDTO.MemberList)
        //     : new List<string>();

        _knownIngredients = roomInfoDTO.KnownIngredientsList != null
            ? roomInfoDTO.KnownIngredientsList.ToHashSet()
            : new HashSet<int>();

        _knownRecipes = roomInfoDTO.KnownRecipesList != null
            ? roomInfoDTO.KnownRecipesList.ToHashSet()
            : new HashSet<int>();
    }
    
    public RoomInfoDTO ToDTO()
    {
        return new RoomInfoDTO(this);
    }

    public RoomInfoNetworkDTO ToNetworkDTO()
    {
        return new RoomInfoNetworkDTO
        {
            ID = ID,
            RoomName = RoomName,
            // MemberCount = MemberCount,
            // MemberList = new List<string>(MemberList),
            KnownIngredientsList = _knownIngredients.ToList(),
            KnownRecipesList = _knownRecipes.ToList()
        };
    }
    
    internal bool AddIngredient(int ingredientID)
    {
        return _knownIngredients.Add(ingredientID);
    }

    internal bool AddRecipe(int recipeID)
    {
        return _knownRecipes.Add(recipeID);
    }
}