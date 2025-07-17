using UnityEngine;

[DefaultExecutionOrder(100)]
public class InventoryPanel_SH : MonoBehaviour
{
    public Transform contentParent;
    public GameObject FoodItemPrefab;

    private void Start()
    {
        foreach (var data in FoodItemManager.Instance.GetAllFoodItems())
        {
            GameObject slot = Instantiate(FoodItemPrefab, contentParent);
            Debug.Log("푸드 아이템 생성되는중");
            FoodItemBehaviour behaviour = slot.GetComponent<FoodItemBehaviour>();
            if (behaviour != null)
            {
                behaviour.Init((FoodItem)data); // AItem을 FoodItem으로 캐스팅
            }
        }
    }
}
