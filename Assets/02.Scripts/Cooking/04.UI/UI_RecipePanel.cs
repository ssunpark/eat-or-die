using System;
using UnityEngine;

public class UI_RecipePanel : MonoBehaviour
{
    public UI_RecipeIngredient UIRecipeIngredient;
    public UI_RecipeList UIRecipeList;
    
    private bool _isInitialized = false;
    

    public void Open()
    {
        gameObject.SetActive(true);

        if (!_isInitialized)
        {
            Init();
            _isInitialized = true;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    
    // private void Start()
    // {
    //     FoodCSVDataManager.Instance.OnDataLoaded += Init;
    //     Debug.Log("UI_RecipePanel Start");
    // }

    private void Init()
    {
        UIRecipeIngredient.Init();
        UIRecipeList.Init();
    }
}
