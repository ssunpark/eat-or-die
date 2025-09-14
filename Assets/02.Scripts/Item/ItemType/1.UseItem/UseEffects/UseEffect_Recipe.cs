using System;
using UnityEngine;

public class UseEffect_Recipe : IUseEffect
{
    public Action<ItemInstance> RecipeScroolUsed;

    public void Use(GameObject target)
    {
        Debug.LogWarning("[UseEffect_Recipe] ItemInstance 없이 호출됨. 오버로드 메서드를 사용하세요.");
    }

    public void Use(GameObject target, ItemInstance itemInstance)
    {
        if (itemInstance == null)
        {
            Debug.LogWarning("[UseEffect_Recipe] ItemInstance가 null입니다.");
            return;
        }

        string recipeIdString = itemInstance.ExtraInfo;
        Debug.Log($"[UseEffect_Recipe] 레시피 스크롤 사용: ItemID={itemInstance.ID}, ExtraInfo={recipeIdString}");

        if (int.TryParse(recipeIdString, out int recipeID))
        {
            // 1. 로컬에서 즉시 팝업 표시 (사용자에게만)
            RecipeShopEvents.InvokeRecipeScrollUsed(recipeID);

            // 2. RPC로 모든 클라이언트에 해금 상태만 동기화 (팝업 없이)
            if (CookingManager.Instance != null && CookingManager.Instance.IsSpawned)
            {
                CookingManager.Instance.RPC_BroadcastRecipeUnlockSync(recipeID);
            }
            else
            {
                Debug.LogWarning("[UseEffect_Recipe] CookingManager가 네트워크에 연결되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[UseEffect_Recipe] ExtraInfo를 레시피 ID로 파싱할 수 없습니다: {recipeIdString}");
        }
    }
}