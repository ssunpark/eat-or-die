using UnityEngine;

public class CraftingTableInteractable : MonoBehaviour, IInteractable
{
    public UI_CraftingTablePanel CraftingTablePanel;
    
    public void Interact()
    {
        Debug.Log("E키 상호작용");
        CraftingTablePanel.Open();
        InputReader.playerControllerInputBlocked = true;
    }
}