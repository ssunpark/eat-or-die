using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftRecipeManager : BehaviourSingleton<CraftRecipeManager>
{
    private const string CRAFTRECIPE_CSV_PATH = "/CraftCSV/CraftRecipe.csv";
    
    public List<CraftRecipe> CraftRecipeList { get; private set; } = new List<CraftRecipe>();
    // public event Action OnDataLoaded;

    private void Awake()
    {
        InitCraftRecipeData();
    }

    private void InitCraftRecipeData()
    {
        CraftRecipeList = CSVLoader<CraftRecipe>.LoadCSV(Application.streamingAssetsPath + CRAFTRECIPE_CSV_PATH);

        Debug.Log($"로드 완료 - CraftRecipeCSVDataList: {CraftRecipeList.Count}, ");
        // OnDataLoaded?.Invoke();
    }

    public List<CraftRecipe> GetAll()
    {
        return CraftRecipeList;
    }
}