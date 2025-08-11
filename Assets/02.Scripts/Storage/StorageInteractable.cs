using Fusion;
using UnityEngine;

public class StorageInteractable : NetworkBehaviour, IInteractable
{
    [SerializeField] private SharedStorage _sharedStorage;

    public bool IsImmediate => true;

    public void Interact()
    {
        Debug.Log("창고를 열어라");
        SharedStorageManager.Instance.OpenStorage(_sharedStorage);
    }
}
