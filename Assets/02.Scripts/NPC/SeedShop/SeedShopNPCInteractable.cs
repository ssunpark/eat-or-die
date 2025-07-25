using UnityEngine;
// 수현
public class SeedShopNPCInteractable : MonoBehaviour, IInteractable
{
    public GameObject SeedShopPanelUI;
    public void Interact()
    {
        SeedShopPanelUI.SetActive(true);
        InputReader.playerControllerInputBlocked = true;
    }
}
