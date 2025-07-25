using System;
using System.Collections.Generic;
using UnityEngine;
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    private const string RECIPE_CSV_PATH = "/FoodCSV/Recipe.csv";
    
    public List<Recipe> RecipeList { get; private set; }
    public event Action OnDataLoaded;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitFoodData();
    }

    private void InitFoodData()
    {
        RecipeList = CSVLoader<Recipe>.LoadCSV(Application.streamingAssetsPath + RECIPE_CSV_PATH);

        Debug.Log($"로드 완료 - RecipeCSVDataList: {RecipeList.Count}, ");
        OnDataLoaded?.Invoke();
    }
}
