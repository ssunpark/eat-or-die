using System;
using UnityEngine;

public class UI_RecipePanel : AUI_PopupBase
{
    public override EPopupType Type =>  EPopupType.Recipe;
    public UI_RecipeIngredient UIRecipeIngredient;
    public UI_RecipeList UIRecipeList;
    
    private bool _isInitialized = false;



    public void Open()
    {
        base.Open();
        if (!_isInitialized)
        {
            Init();
            _isInitialized = true;
        }
    }

    public void Close()
    {
        base.Close();
    }

    private void Init()
    {
        UIRecipeIngredient.Init();
        UIRecipeList.Init();
    }
}
