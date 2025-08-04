using Fusion;
using UnityEngine;
// 수현
public class CookingPotInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;
    public UI_CookingPanel CookingPanelUI;
    public void Interact()
    {
        CookingPanelUI.Open();
        InputReader.Instance.ReleaseControl();
    }
}
