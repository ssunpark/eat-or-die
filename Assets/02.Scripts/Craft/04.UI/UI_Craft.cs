using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    public GameObject CraftingTablePanel; 
    private void Start()
    {
        CraftingTablePanel.SetActive(false);
    }
}
