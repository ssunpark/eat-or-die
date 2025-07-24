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

    // private bool _isDespawn;
    private NetworkId _targetID;
    private bool _hasOwner;
    public bool HasOwner { get => _hasOwner; set => _hasOwner = value; }

    [SerializeField]
    private float _absorbSpeed = 10f;
    [SerializeField]
    private float _absorbThreshold = 0.1f;

    // 흡수 대상
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
        if (_target != null)
        {
            transform.position = Vector3.Lerp(transform.position, _target.position, _absorbSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target.position) < _absorbThreshold)
            {
                // 인벤에 등록 요청
                if (_target.GetComponent<NetworkObject>().HasInputAuthority)
                {
                    var itemData = ItemManager.Instance.GetItem(ItemID).ItemData;
                    Item item = new Item(ItemID,
                        itemData.MaxQuantity, Quantity, itemData.MaxDurability, Durability);
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
        {
            return;
        }

        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Pick(NetworkId targetNetworkId)
    {
        _hasOwner = true;

        // 이 아이템을 주운 경우 줍기 비활성화
        _collider.enabled = false;

        // 아이템 흡수 연출 타겟 설정
        _targetID = targetNetworkId;
        _target = Runner.FindObject(targetNetworkId).gameObject.transform;
    }

    // 외형 적용
    private void ApplyVisual(Sprite icon)
    {
        _spriteRenderer.sprite = icon;
    }
}