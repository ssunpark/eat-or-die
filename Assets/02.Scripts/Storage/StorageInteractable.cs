using UnityEngine;

public class StorageInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] SharedStorage _sharedStorage;
    
    public bool IsImmediate => true;

    public void Interact()
    {
        Debug.Log("창고를 열어라");
        SharedStorageManager.Instance.OpenStorage(_sharedStorage);
    }
}
