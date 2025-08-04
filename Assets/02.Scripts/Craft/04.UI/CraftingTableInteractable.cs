using UnityEngine;

public class CraftingTableInteractable : MonoBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public UI_CraftingTablePanel CraftingTablePanel;
    
    public void Interact()
    {
        Debug.Log("E키 상호작용");
        CraftingTablePanel.Open();
        InputReader.Instance.ReleaseControl();
    }
}