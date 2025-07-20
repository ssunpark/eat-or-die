using System.Collections.Generic;
using UnityEngine;
public class FoodCSVDataManager : MonoBehaviour
{
    public static FoodCSVDataManager Instance { get; private set; }

    private const string RECIPE_CSV_PATH = "/FoodCSV/FoodCSV.csv";
    private const string INGREDIENT_CSV_PATH = "/FoodCSV/IngredientCSV.csv";
    // public List<FoodCSVData> AllDataList { get; private set; } = new List<FoodCSVData>();
    // public List<FoodCSVData> HarvestDataList { get; private set; } = new List<FoodCSVData>();
    // public List<FoodCSVData> BuffDataList { get; private set; } = new List<FoodCSVData>();
    
    public List<RecipeCSVData> RecipeCSVDataList { get; private set; }
    public List<IngredientCSVData> IngredientCsvDataList { get; private set; }
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
        RecipeCSVDataList = CSVLoader<RecipeCSVData>.LoadFoodCSV(Application.streamingAssetsPath + RECIPE_CSV_PATH);
        IngredientCsvDataList = CSVLoader<IngredientCSVData>.LoadFoodCSV(Application.streamingAssetsPath + INGREDIENT_CSV_PATH);
        // HarvestDataList = AllDataList.FindAll(x => x.ERecipeType == "Harvest");
        // BuffDataList = AllDataList.FindAll(x => x.ERecipeType == "Buff");

        Debug.Log($"로드 완료 - RecipeCSVDataList: {RecipeCSVDataList.Count}, IngredientCsvDataList: {IngredientCsvDataList.Count}");
    }

    // public FoodCSVData GetFoodDataByID(int id)
    // {
    //     return AllDataList.Find(x => x.ID == id);
    // }
}
