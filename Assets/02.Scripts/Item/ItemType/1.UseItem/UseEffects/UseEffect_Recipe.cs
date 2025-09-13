using System;
using UnityEngine;

public class UseEffect_Recipe : IUseEffect
{
    private static ItemInstance _currentItem;

    public static void SetCurrentItem(ItemInstance item)
    {
        _currentItem = item;
        Debug.Log($"[UseEffect_Recipe] 현재 아이템 설정: ExtraInfo = '{item.ExtraInfo}'");
    }

    public void Use(GameObject target)
    {
        Debug.Log("[UseEffect_Recipe] Use 메서드 실행");

        if (_currentItem == null)
        {
            Debug.LogWarning("[UseEffect_Recipe] 현재 아이템 정보가 없습니다.");
            return;
        }

        string recipeIdString = _currentItem.ExtraInfo;
        Debug.Log($"[UseEffect_Recipe] ExtraInfo에서 레시피 ID 추출: '{recipeIdString}'");

        if (int.TryParse(recipeIdString, out int recipeID))
        {
            RecipeShopEvents.InvokeRecipeScrollUsed(recipeID);
            Debug.Log($"[UseEffect_Recipe] 레시피 스크롤 사용! 해금 시도: Recipe ID {recipeID}");
        }
        else
        {
            Debug.LogWarning($"[UseEffect_Recipe] 유효하지 않은 레시피 정보: '{recipeIdString}'");
        }

        _currentItem = null;
    }
}