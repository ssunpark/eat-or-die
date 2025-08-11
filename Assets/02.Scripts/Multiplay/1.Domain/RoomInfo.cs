using System;
using System.Collections.Generic;
using System.Linq;
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

    public RoomInfo()
    {
        _knownIngredients = new HashSet<int>();
        _knownRecipes = new HashSet<int>();
    }

    public RoomInfo(RoomInfoDTO roomInfoDTO)
    {
        RoomName = roomInfoDTO.RoomName;
        _knownIngredients = roomInfoDTO.KnownIngredientsList.ToHashSet();
        _knownRecipes = roomInfoDTO.KnownRecipesList.ToHashSet();
    }
    public RoomInfoDTO ToDTO()
    {
        return new RoomInfoDTO(this);
    }

    // 나중에 따로 빼겠음
    internal bool AddIngredient(int ingredientID)
    {
        return _knownIngredients.Add(ingredientID);
    }

    internal bool AddRecipe(int recipeID)
    {
        return _knownRecipes.Add(recipeID);
    }
}