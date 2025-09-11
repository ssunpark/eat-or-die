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
        return RecipeList.Where(recipe =>
        {
            ItemProfile resultItemProfile = ItemManager.Instance.GetItem(recipe.ResultID);
            
            if (resultItemProfile == null)
            {
                return false;
            }
            
            EItemType resultItemType = resultItemProfile.ItemDefinition.Type;
            
            switch (category)
            {
                case ERecipeCategory.Food:
                    return resultItemType == EItemType.Food;
                case ERecipeCategory.Weapon:
                    return resultItemType == EItemType.Weapon;
                default:
                    return false;
            }
        }).ToList();
    }
}
