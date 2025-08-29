using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class StorageInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private SharedStorage _sharedStorage;

    public bool IsImmediate => true;

    public float InteractionDistanceOffset => 1f;

    Player IInteractable.InteractingPlayer => _interactingPlayer;
    private Player _interactingPlayer;

    public void Interact()
    {
        SharedStorageManager.Instance.RegisterStorage(_sharedStorage);
        MasterAudio.PlaySound3DAtTransform("ChestOpen", transform);
    }

    void IInteractable.Interact(Player from)
    {
        _interactingPlayer = from;
        Interact();
    }
}
