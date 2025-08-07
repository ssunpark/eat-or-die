using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 수현
[Serializable]
public class RoomInfoDTO
{
    public string RoomName;
    public List<int> KnownIngredientsList = new List<int>();
    public List<int> KnownRecipesList = new List<int>();

    public static RoomInfoDTO FromDomain(RoomInfo roomInfo)
    {
        if (roomInfo == null)
            Debug.Log("roomInfo 자체가 null입니다!");
        
        if(roomInfo.RoomName == null)
            Debug.Log("RoomName이 널입니다.");
        
        if (roomInfo.KnownIngredients == null)
            Debug.Log("roomInfo.KnownIngredients가 null입니다!");

        if (roomInfo.KnownRecipes == null)
            Debug.Log("roomInfo.KnownRecipes가 null입니다!");
        
        return new RoomInfoDTO()
        {
            RoomName = roomInfo.RoomName,
            KnownIngredientsList = roomInfo.KnownIngredients.ToList(),
            KnownRecipesList = roomInfo.KnownRecipes.ToList()
        };
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(
            RoomName,
            new HashSet<int>(KnownIngredientsList ?? new List<int>()),
            new HashSet<int>(KnownRecipesList ?? new List<int>())
            );
    }
}
