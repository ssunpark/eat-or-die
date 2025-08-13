using Fusion;
using UnityEngine;

public class CraftingTableInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public UI_CraftPanel craftPanel;
    
    public void Interact()
    {
        Debug.Log("E키 상호작용");
        craftPanel.Open();
        InputReader.Instance.ReleaseControl();
    }
}