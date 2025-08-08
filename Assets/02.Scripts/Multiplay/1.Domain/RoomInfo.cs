using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInfo
{
    // 방 기준으로 저장해야 하는 리스트들을 여기에 정의.
    [Header("방 명세")]
    public string RoomName;

    // 외부에서는 읽기만 가능한 IReadOnlyCollection<T>으로 노출
    [Header("요리 시스템")]
    public IReadOnlyCollection<int> KnownIngredients => _knownIngredients;
    public IReadOnlyCollection<int> KnownRecipes => _knownRecipes;
    
    // 실제 데이터는 private 필드로 관리. [SerializeField]로 인스펙터에는 보이지만 외부 코드에서는 접근 불가.
    [SerializeField] private HashSet<int> _knownIngredients;
    [SerializeField] private HashSet<int> _knownRecipes;

    // 기본 생성자 (빈 상태로 초기화)
    public RoomInfo()
    {
        RoomName = "TestRoom";
        _knownIngredients = new HashSet<int>();
        _knownRecipes = new HashSet<int>();
    }

    // 명시적으로 상태를 전달받는 생성자(DTO 변환 시 사용)
    public RoomInfo(string roomName, HashSet<int> knownIngredients, HashSet<int> knownRecipes)
    {
        RoomName = roomName ?? "Unnamed Room";
        _knownIngredients = knownIngredients ?? new HashSet<int>();
        _knownRecipes = knownRecipes ?? new HashSet<int>();;
    }
    
    // --- 데이터 수정을 위한 내부(internal) 메서드 추가 ---
    // 이제 RoomInfo 데이터 수정은 이 메서드를 통해서만 가능.
    // internal은 같은 어셈블리(프로젝트) 내에서만 접근 가능.
    internal bool AddIngredient(int ingredientID)
    {
        return _knownIngredients.Add(ingredientID);
    }

    internal bool AddRecipe(int recipeID)
    {
        return _knownRecipes.Add(recipeID);
    }
    
}