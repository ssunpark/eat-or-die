using System;
using System.Collections.Generic;
[Serializable]
public class RoomInfo
{
    public HashSet<int> KnownIngredients;

    public RoomInfo(HashSet<int> knownIngredients)
    {
        KnownIngredients = knownIngredients;
    }
}