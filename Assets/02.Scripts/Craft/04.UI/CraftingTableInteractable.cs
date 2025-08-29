using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class CraftingTableInteractable : NetworkBehaviour, IInteractable
{
    public bool IsImmediate => true;

    public float InteractionDistanceOffset => 1f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;
    private Player _interactingPlayer;

    public UI_CraftPanel craftPanel;
    
    public void Interact()
    {
        Debug.Log("E키 상호작용");
        craftPanel.Open();
        MasterAudio.PlaySound3DAtTransform("CraftCompleted", transform);
    }

    void IInteractable.Interact(Player from)
    {
        Interact();
    }
}