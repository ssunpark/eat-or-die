using UnityEngine;
// 수현
public class SeedShopNpcInteractable : MonoBehaviour, IInteractable
{
    public UI_SeedShopPanel UI_SeedShopPanel;
    public GameObject SeedShopPanelUI;
    public void Interact()
    {
        UI_SeedShopPanel.Open();
        InputReader.playerControllerInputBlocked = true;
    }
}
