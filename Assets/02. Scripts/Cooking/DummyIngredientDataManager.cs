using System.Collections.Generic;
using UnityEngine;
//수현
public class DummyIngredientDataManager : BehaviourSingleton<DummyIngredientDataManager>
{
    public List<IngredientData> DummyIngredientDatas = new List<IngredientData>();

    private void Start()
    {
        DummyIngredientDatas = new List<IngredientData>
        {
            new IngredientData { ID = 100, Name = "빨간 버섯" },
            new IngredientData { ID = 101, Name = "파란 버섯" },
            new IngredientData { ID = 102, Name = "고기" },
            new IngredientData { ID = 103, Name = "고추" },
            new IngredientData { ID = 104, Name = "차가운 풀잎" }
        };
    }

    public IngredientData GetIngredientByID(int id)
    {
        return DummyIngredientDatas.Find(i => i.ID == id);
    }
}
