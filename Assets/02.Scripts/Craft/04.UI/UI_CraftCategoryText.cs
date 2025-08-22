using TMPro;
using UnityEngine;

public class UI_CraftCategoryText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _categoryTextUI;

    public void OnClickAllCategory()
    {
        _categoryTextUI.text = "전체";
    }

    public void OnClickToolCategory()
    {
        _categoryTextUI.text = "도구";
    }

    public void OnClickWeaponCategory()
    {
        _categoryTextUI.text = "무기";
    }

    public void OnClickEquipmentCategory()
    {
        _categoryTextUI.text = "장비";
    }
}