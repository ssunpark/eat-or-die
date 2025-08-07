using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    public HashSet<int> KnownIngredients = new HashSet<int>();

    public RoomInfo(HashSet<int> knownIngredients)
    {
        KnownIngredients = knownIngredients;
    }
}