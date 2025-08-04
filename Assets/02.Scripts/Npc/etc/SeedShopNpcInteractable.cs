using Fusion;
using UnityEngine;
// 수현
public class SeedShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public UI_SeedShopPanel UI_SeedShopPanel;
    public void Interact()
    {
        UI_SeedShopPanel.Open();
        InputReader.Instance.GainControl();
    }
}
