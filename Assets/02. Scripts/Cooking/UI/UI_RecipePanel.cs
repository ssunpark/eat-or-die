using System;
using UnityEngine;

public class UI_RecipePanel : MonoBehaviour
{
    public UI_RecipeIngredient UIRecipeIngredient;
    public UI_RecipeList UIRecipeList;
    
    private void Start()
    {
        FoodCSVDataManager.Instance.OnDataLoaded += Init;
    }

    private void Init()
    {
        UIRecipeIngredient.Init();
        UIRecipeList.Init();
    }
}
