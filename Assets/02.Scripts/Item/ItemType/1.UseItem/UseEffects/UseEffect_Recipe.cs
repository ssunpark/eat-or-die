using System;
using UnityEngine;

public class UseEffect_Recipe : IUseEffect
{
    private static ItemInstance _currentItem;
    public Action<ItemInstance> RecipeScroolUsed;

    public static void SetCurrentItem(ItemInstance item)
    {
        _currentItem = item;
    }

    public void Use(GameObject target)
    {
        if (_currentItem == null)
        {
            return;
        }

        string recipeIdString = _currentItem.ExtraInfo;

        if (int.TryParse(recipeIdString, out int recipeID))
        {
            RecipeShopEvents.InvokeRecipeScrollUsed(recipeID);
        }

        _currentItem = null;
    }
}