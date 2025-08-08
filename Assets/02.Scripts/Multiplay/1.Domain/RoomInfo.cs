using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    // 방 기준으로 저장해야 하는 리스트들을 여기에 정의.
    [Header("방 명세")]
    public string RoomName;

    [Header("요리 시스템")]
    public HashSet<int> KnownIngredients;
    public HashSet<int> KnownRecipes;

    // 기본 생성자 (빈 상태로 초기화)
    public RoomInfo()
    {
        RoomName = "RoomInfo Test Room";
        KnownIngredients = new HashSet<int>();
        KnownRecipes = new HashSet<int>();
    }

    // 명시적으로 상태를 전달받는 생성자(DTO 변환 시 사용)
    public RoomInfo(string roomName, HashSet<int> knownIngredients, HashSet<int> knownRecipes)
    {
        RoomName = roomName ?? "Unnamed Room";
        KnownIngredients = knownIngredients ?? new HashSet<int>();
        KnownRecipes = knownRecipes ?? new HashSet<int>();;
    }
}