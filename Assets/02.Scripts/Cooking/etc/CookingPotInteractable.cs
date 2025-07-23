using UnityEngine;
// 수현
public class CookingPotInteractable : MonoBehaviour, IInteractable
{
    public GameObject CookingPanelUI;
    public void Interact()
    {
        CookingPanelUI.SetActive(true);
        InputReader.playerControllerInputBlocked = true;
    }
}
