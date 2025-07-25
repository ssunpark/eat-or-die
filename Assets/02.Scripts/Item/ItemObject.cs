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
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider>();
    }

    public override void Spawned()
    {
        transform.position = SpawnPosition;
        var icon = ItemManager.Instance.GetItem(ItemID).ItemData.Icon;
        ApplyVisual(icon);
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
                    var itemData = ItemManager.Instance.GetItem(ItemID).ItemData;
                    var item = new Item(ItemID, itemData.MaxQuantity, Quantity, itemData.MaxDurability, Durability, ExtraInfo);
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

    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}