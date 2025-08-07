using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    public HashSet<int> KnownIngredients { get; private set; }
    public HashSet<int> KnownRecipes { get; private set; }

    // 기본 생성자 (빈 상태로 초기화)
    public RoomInfo()
    {
        KnownIngredients = new HashSet<int>();
        KnownRecipes = new HashSet<int>();
    }

    // 명시적으로 상태를 전달받는 생성자(DTO 변환 시 사용)
    public RoomInfo(HashSet<int> knownIngredients, HashSet<int> knownRecipes)
    {
        KnownIngredients = knownIngredients ?? new HashSet<int>();
        KnownRecipes = knownRecipes ?? new HashSet<int>();;
    }
}