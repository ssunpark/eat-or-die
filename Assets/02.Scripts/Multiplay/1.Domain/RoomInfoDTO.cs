using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 수현
[Serializable]
public class RoomInfoDTO
{
    public List<int> KnownIngredientsList = new List<int>();
    public List<int> KnownRecipesList = new List<int>();

    public static RoomInfoDTO FromDomain(RoomInfo roomInfo)
    {
        return new RoomInfoDTO()
        {
            KnownIngredientsList = roomInfo.KnownIngredients.ToList(),
            KnownRecipesList = roomInfo.KnownRecipes.ToList()
        };
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(
            new HashSet<int>(KnownIngredientsList ?? new List<int>()),
            new HashSet<int>(KnownRecipesList ?? new List<int>())
            );
    }
}
