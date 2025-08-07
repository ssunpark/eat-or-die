using System;
using System.Collections.Generic;
using UnityEngine;
// 수현
[Serializable]
public class RoomInfoDTO
{
    public List<int> KnownIngredientsList = new List<int>();

    public static RoomInfoDTO FromDomain(RoomInfo roomInfo)
    {
        return new RoomInfoDTO()
        {
            KnownIngredientsList = new List<int>(roomInfo.KnownIngredients)
        };
    }

    public RoomInfo ToDomain()
    {
        return new RoomInfo(new HashSet<int>(KnownIngredientsList));
    }
}
