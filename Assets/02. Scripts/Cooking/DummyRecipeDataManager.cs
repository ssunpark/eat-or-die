using System.Collections.Generic;
using UnityEngine;
//수현
public class DummyRecipeDataManager : BehaviourSingleton<DummyRecipeDataManager>
{
    public List<RecipeData> DummyRecipeDatas = new List<RecipeData>();

    private void Start()
    {
        CreateDummyDatas();
    }

    private void CreateDummyDatas()
    {
        DummyRecipeDatas.Add(new RecipeData { ID = 1, Name = "회복 스프", Ingredient1ID = 100, Ingredient2ID = 101 });
        DummyRecipeDatas.Add(new RecipeData { ID = 2, Name = "힘의 찜", Ingredient1ID = 102, Ingredient2ID = 103 });
        DummyRecipeDatas.Add(new RecipeData { ID = 3, Name = "냉기의 수프", Ingredient1ID = 104, Ingredient2ID = 105 });
        DummyRecipeDatas.Add(new RecipeData { ID = 4, Name = "불꽃 카레", Ingredient1ID = 106, Ingredient2ID = 107 });     // 공격력 버프
        DummyRecipeDatas.Add(new RecipeData { ID = 5, Name = "민첩의 샐러드", Ingredient1ID = 108, Ingredient2ID = 109 });  // 이동속도 증가
        DummyRecipeDatas.Add(new RecipeData { ID = 6, Name = "맵고 신 요리", Ingredient1ID = 110, Ingredient2ID = 111 });   // 디버프용 요리
        DummyRecipeDatas.Add(new RecipeData { ID = 7, Name = "바위 찜", Ingredient1ID = 112, Ingredient2ID = 113 });        // 방어력 증가
        DummyRecipeDatas.Add(new RecipeData { ID = 8, Name = "빛나는 샐러드", Ingredient1ID = 114, Ingredient2ID = 115 });  // 특수효과용
        DummyRecipeDatas.Add(new RecipeData { ID = 9, Name = "강철 스튜", Ingredient1ID = 116, Ingredient2ID = 117 });      // 무기강화용
    }

    public RecipeData GetRecipeByID(int id)
    {
        return DummyRecipeDatas.Find(r => r.ID == id);
    }
}
