using DarkTonic.MasterAudio;
using Fusion;
using UnityEngine;

public class StorageInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private SharedStorage _sharedStorage;

    public bool IsImmediate => true;

    public float InteractionDistanceOffset => 1f;

    public void Interact()
    {
        SharedStorageManager.Instance.RegisterStorage(_sharedStorage);
        MasterAudio.PlaySound3DAtTransform("ChestOpen", transform);
    }
}
