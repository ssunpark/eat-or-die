using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : BehaviourSingleton<RecipeManager>
{
    private const string RECIPE_CSV_PATH = "/FoodCSV/Recipe.csv";
    
    public List<Recipe> RecipeList { get; private set; }

    private void Awake()
    {
        InitFoodData();
        DontDestroyOnLoad(gameObject);
    }

    private void InitFoodData()
    {
        RecipeList = CSVLoader<Recipe>.LoadCSV(Application.streamingAssetsPath + RECIPE_CSV_PATH);

        Debug.Log($"로드 완료 - RecipeCSVDataList: {RecipeList.Count}, ");
    }
    
    public List<Recipe> GetRecipesByCategory(ERecipeCategory category)
    {
        // 전체 레시피 리스트에서 조건에 맞는 것만 필터링합니다.
        return RecipeList.Where(recipe =>
        {
            // 1. 레시피의 결과물 아이템 정보를 ItemManager에서 가져옵니다.
            ItemProfile resultItemProfile = ItemManager.Instance.GetItem(recipe.ResultID);

            // 2. 결과물 아이템이 존재하지 않으면 필터에서 제외합니다.
            if (resultItemProfile == null)
            {
                return false;
            }

            // 3. 결과물 아이템의 타입을 가져옵니다.
            EItemType resultItemType = resultItemProfile.ItemDefinition.Type;

            // 4. 요청된 카테고리와 결과물 아이템의 타입을 비교하여 일치하는지 확인합니다.
            switch (category)
            {
                case ERecipeCategory.Food:
                    return resultItemType == EItemType.Food;
                case ERecipeCategory.Weapon:
                    return resultItemType == EItemType.Weapon;
                default:
                    return false;
            }
        }).ToList(); // 5. 필터링된 결과를 새로운 리스트로 만들어 반환합니다.
    }
}
