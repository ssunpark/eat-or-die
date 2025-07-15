using UnityEngine;
using UnityEngine.UI;
public class IngredientItemHandler : MonoBehaviour
{
    public IngredientData data;
    public Image iconImage;

    public void Init(IngredientData newData)
    {
        data = newData;
        iconImage.sprite = data.Icon;
    }

}
