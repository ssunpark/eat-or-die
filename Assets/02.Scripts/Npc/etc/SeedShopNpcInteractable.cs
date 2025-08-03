using Fusion;
using UnityEngine;
// 수현
public class SeedShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public UI_SeedShopPanel UI_SeedShopPanel;
    public void Interact()
    {
        UI_SeedShopPanel.Open();
        InputReader.playerControllerInputBlocked = true;
    }
}
