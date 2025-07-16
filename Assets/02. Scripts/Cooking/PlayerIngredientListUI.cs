using UnityEngine;
public class PlayerIngredientListUI : MonoBehaviour
{
    public Transform ingredientListParent;
    public GameObject ingredientItemPrefab;

    private void Start()
    {
        GenerateIngredientList();
    }

    public void GenerateIngredientList()
    {
        foreach (var data in DummyIngredientDataManager.Instance.DummyIngredientDatas)
        {
            GameObject item = Instantiate(ingredientItemPrefab, ingredientListParent);
            item.GetComponent<IngredientItemHandler>().Init(data);
        }
    }
}
