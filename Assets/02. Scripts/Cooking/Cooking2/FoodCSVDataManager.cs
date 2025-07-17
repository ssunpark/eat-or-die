using System.Collections.Generic;
using UnityEngine;
public class FoodCSVDataManager : MonoBehaviour
{
    public static FoodCSVDataManager Instance { get; private set; }

    private const string FOOD_CSV_PATH = "/FoodCSV/FoodCSV3.csv";

    public List<FoodCSVData> AllDataList { get; private set; } = new List<FoodCSVData>();
    public List<FoodCSVData> HarvestDataList { get; private set; } = new List<FoodCSVData>();
    public List<FoodCSVData> BuffDataList { get; private set; } = new List<FoodCSVData>();

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
        AllDataList = FoodCSVLoader.LoadFoodCSV(Application.streamingAssetsPath + FOOD_CSV_PATH);
        HarvestDataList = AllDataList.FindAll(x => x.ERecipeType == "Harvest");
        BuffDataList = AllDataList.FindAll(x => x.ERecipeType == "Buff");

        Debug.Log($"로드 완료 - 전체: {AllDataList.Count}, Harvest: {HarvestDataList.Count}, Buff: {BuffDataList.Count}");
    }

    public FoodCSVData GetFoodDataByID(int id)
    {
        return AllDataList.Find(x => x.ID == id);
    }
}
