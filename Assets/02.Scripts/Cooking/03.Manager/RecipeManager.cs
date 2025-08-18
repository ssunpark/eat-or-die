using System;
using System.Collections.Generic;
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
}
