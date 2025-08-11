using Fusion;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : NetworkBehaviour, IPickable
{
    [Networked]
    public int ItemID { get; set; }

    [Networked]
    public int Quantity { get; set; }

    [Networked]
    public float Durability { get; set; }

    [Networked]
    public Vector3 SpawnPosition { get; set; }

    [Networked]
    public bool HasNetworkedOwner { get; set; }
    
    [Networked, Capacity(24)]
    public string ExtraInfo { get; set; }

    private NetworkId _targetID;

    private float _time;
    
    private bool _isPickable;
    public bool IsPickable => _isPickable;
    
    private bool _hasOwnerLocal;
    public bool HasOwnerLocal { get => _hasOwnerLocal; set => _hasOwnerLocal = value; }

    [SerializeField]
    private float _absorbSpeed = 10f;
    [SerializeField]
    private float _absorbThreshold = 0.1f;
    [SerializeField]
    private float _pickableTime = 1f;

    private Transform _target;
    private Collider _collider;

    private GameObject _itemObject;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public override void Spawned()
    {
        transform.position = SpawnPosition;
        _itemObject = ItemManager.Instance.GetItem(ItemID).GetHoldItemObject();
        ApplyVisual();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time >= _pickableTime)
        {
            _isPickable = true;
        }

        if (!_isPickable)
            return;

        if (_target != null)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position, _absorbSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target.position) < _absorbThreshold)
            {
                if (_target.GetComponent<NetworkObject>().HasInputAuthority)
                {
                    var itemData = ItemManager.Instance.GetItem(ItemID);
                    itemData.ReturnHoldItemToPool(_itemObject);
                    var item = new ItemInstance(itemData, Quantity, Durability, ExtraInfo);
                    InventoryManager.Instance.PickItemFromGround(item);
                    RPC_Despawn();
                }

                _target = null;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Despawn()
    {
        if (!Runner.IsServer)
            return;

        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Pick(NetworkId targetNetworkId)
    {
        _collider.enabled = false;
        _targetID = targetNetworkId;
        _target = Runner.FindObject(targetNetworkId)?.transform;
    }

    private void ApplyVisual()
    {
        _itemObject.transform.SetParent(transform);
        _itemObject.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        // _itemObject.transform.localRotation = Quaternion.identity;
        
        NormalizeVisualScale(_itemObject, 1f);
    }
    
    private void NormalizeVisualScale(GameObject obj, float targetSize)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds combinedBounds = new Bounds(obj.transform.position, Vector3.zero);

        foreach (var renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        float largestDimension = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);
        float scaleFactor = targetSize / largestDimension;

        obj.transform.localScale *= scaleFactor;
    }
}