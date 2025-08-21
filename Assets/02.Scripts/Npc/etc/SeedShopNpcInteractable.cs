using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;
// 수현
public class SeedShopNpcInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;

    public float InteractionDistanceOffset => 0.5f;

    public UI_SeedShopPanel UI_SeedShopPanel;
    public void Interact()
    {
        UI_SeedShopPanel.Open();
        InputReader.Instance.ReleaseControl();
        MasterAudio.PlaySound3DAtTransform("NpcInteract", transform);
    }
}
