using UnityEngine;
public class InventoryPanel_SH : MonoBehaviour
{
    public Transform contentParent;
    public GameObject itemSlotPrefab;

    private void Start()
    {
        foreach (var data in FoodItemManager.Instance.GetAllFoodItems())
        {
            GameObject slot = Instantiate(itemSlotPrefab, contentParent);
        }
    }
}
