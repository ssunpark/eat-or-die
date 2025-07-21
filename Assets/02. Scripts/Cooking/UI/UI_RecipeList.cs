using System.Collections.Generic;
using UnityEngine;

public class UI_RecipeList : MonoBehaviour
{
    public GameObject Container;
    public GameObject ButtonPrefab;

    private List<RecipeCSVData> _recipeCsvDataList;

    // CSV 불러와서 버튼 생성 - 초기화
    public void Init()
    {
        _recipeCsvDataList = FoodCSVDataManager.Instance.RecipeCSVDataList;

        foreach (Transform child in Container.transform)
        {
            Destroy(child.gameObject); // 기존 버튼들 제거 (선택 사항)
        }

        foreach (var recipeData in _recipeCsvDataList)
        {
            GameObject buttonObj = Instantiate(ButtonPrefab, Container.transform);
            // 필요하다면 버튼에 데이터 바인딩 처리
            예: buttonObj.GetComponent<UI_RecipeButton>().Refresh(recipeData);
        }
    }
}