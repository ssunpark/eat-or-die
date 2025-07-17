using System.Collections.Generic;
using UnityEngine;

public class FoodItemManager : MonoBehaviour
{
    public static FoodItemManager Instance { get; private set; }

    private Dictionary<int, AItem> _foodItemDict = new Dictionary<int, AItem>();
    private FoodItemFactory _factory = new FoodItemFactory();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (FoodCSVData data in FoodCSVDataManager.Instance.AllDataList)
        {
            AItem foodItem = _factory.CreateFoodItem(data);
            _foodItemDict[data.ID] = foodItem;
        }
        Debug.Log($"FoodItemManager : {_foodItemDict.Count}개 등록됨");
    }

    public AItem GetFoodItem(int id)
    {
        if (_foodItemDict.TryGetValue(id, out AItem item))
        {
            return item;
        }
        Debug.LogError($"없는 FoodItem ID : {id}");
        return null;
    }

    public IEnumerable<AItem> GetAllFoodItems()
    {
        return _foodItemDict.Values;
    }
}