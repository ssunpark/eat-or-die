using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CookingCompletedPopup : MonoBehaviour
{
    public GameObject CookingCompletedPopupUI;
    public Image RecipeIcon;
    public TextMeshProUGUI RecipeName;
    public TextMeshProUGUI RecipeDescription;

    private void Start()
    {
        CookingCompletedPopupUI.SetActive(false);
    }
    public void Refresh(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("[UICookingCompletedPopup] itemData is null");
            return;
        }

        RecipeIcon.sprite = itemData.Icon;
        RecipeName.text = itemData.Name;
        RecipeDescription.text = itemData.Description;
    }
}