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

    public RoomInfoDTO (RoomInfo roomInfo)
    {
        RoomName = roomInfo.RoomName;
        KnownIngredientsList = roomInfo.KnownIngredients.ToList();
        KnownRecipesList = roomInfo.KnownRecipes.ToList();
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
