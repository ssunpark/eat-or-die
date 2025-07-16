using System.Collections.Generic;
using UnityEngine;
//수현
public class DummyIngredientDataManager : MonoBehaviour
{
    public static DummyIngredientDataManager Instance { get; private set; }

    public List<IngredientData> DummyIngredientDatas = new List<IngredientData>();

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
        CreateDummyDatas();
    }

    private void CreateDummyDatas()
    {
        DummyIngredientDatas.Add(new IngredientData{ID = 100, Name = "빨간 버섯"});
        DummyIngredientDatas.Add(new IngredientData{ID = 101, Name = "파란 버섯"});
        DummyIngredientDatas.Add(new IngredientData{ID = 102, Name = "고기"});
        DummyIngredientDatas.Add(new IngredientData{ID = 103, Name = "고추"});
        DummyIngredientDatas.Add(new IngredientData{ID = 104, Name = "차가운 풀잎"});
        DummyIngredientDatas.Add(new IngredientData{ID = 105, Name = "미지근한 풀잎"});
        DummyIngredientDatas.Add(new IngredientData{ID = 106, Name = "영롱한 나뭇가지"});
        DummyIngredientDatas.Add(new IngredientData{ID = 107, Name = "노란 열매"});
        DummyIngredientDatas.Add(new IngredientData{ID = 108, Name = "파란 열매"});
    }

    public IngredientData GetIngredientByID(int id)
    {
        return DummyIngredientDatas.Find(i => i.ID == id);
    }
}
