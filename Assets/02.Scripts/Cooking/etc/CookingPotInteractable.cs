using Fusion;
using UnityEngine;
// 수현
public class CookingPotInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public GameObject CookingPanelUI;
    public void Interact()
    {
        CookingPanelUI.SetActive(true);
        InputReader.playerControllerInputBlocked = true;
    }
}
