using DG.Tweening;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

// 게임 내 보여지는 아이템 오브젝트
public class ItemObject : NetworkBehaviour, IPickable
{
    private const string PARTICLE_KEY = "Item_Effect";
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

    [Header("아이템 표준 크기")]
    [SerializeField]
    private float _normalizeSize = 1f;
    [Header("아이템 흡수")]
    [SerializeField]
    private float _absorbSpeed = 10f;
    [SerializeField]
    private float _absorbThreshold = 0.1f;
    [Networked]
    public float PickableTime { get; set; } = 4f;

    private Transform _target;
    private Collider _collider;

    private GameObject _itemObject;

    private Quaternion _itemRotationSnapShot;
    private Vector3 _itemPositionSnapShot;
    private ParticleSystem _itemParticle;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public override void Spawned()
    {
        transform.position = SpawnPosition + (Vector3.up * 0.5f);
        _itemObject = ItemManager.Instance.GetItem(ItemID).GetHoldItemObject();
        _itemParticle = ParticleManager.Instance.PlayByKeyLocalAsChild(PARTICLE_KEY, transform, Vector3.zero, quaternion.identity);
        ApplyVisual();
        
        StartFloatingAndRotating();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time >= PickableTime)
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
                    _target = null;
                    var itemProfile = ItemManager.Instance.GetItem(ItemID);
                    ReturnItemToPool(itemProfile);
                    var item = new ItemInstance(itemProfile, Quantity, Durability, ExtraInfo);
                    UnifiedInventoryManager.Instance.AddItem(item);
                    RPC_Despawn();
                }

                _target = null;
            }
        }
    }
    
    private void StartFloatingAndRotating()
    {
        // 기준 위치
        Vector3 startPos = _itemObject.transform.position;

        // 위아래 이동 (Y축)
        _itemObject.transform.DOMoveY(startPos.y + 1f, 1f) // 위로 1 높이 이동
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // 회전 (Y축 기준)
        _itemObject.transform.DORotate(new Vector3(0f, 360f, 0f), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
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
        _itemObject.transform.DOKill();
        _collider.enabled = false;
        _targetID = targetNetworkId;
        _target = Runner.FindObject(targetNetworkId)?.transform;
        _itemParticle.Stop();
    }

    private void ApplyVisual()
    {
        _itemObject.transform.SetParent(transform);
        _itemObject.transform.localPosition = Vector3.zero;
        _itemObject.transform.localRotation = Quaternion.identity;
        
        var actualObject = _itemObject.transform.GetChild(0);
        _itemRotationSnapShot = actualObject.localRotation; // 원본 회전을 기록
        _itemPositionSnapShot = actualObject.localPosition;
        actualObject.localRotation = Quaternion.identity;
        actualObject.localPosition = Vector3.zero;
        
        NormalizeVisualScale(_itemObject, _normalizeSize);
    }

    private void ReturnItemToPool(ItemProfile itemProfile)
    {
        _itemObject.transform.GetChild(0).localPosition = _itemPositionSnapShot;
        _itemObject.transform.GetChild(0).localRotation = _itemRotationSnapShot;
        itemProfile.ReturnHoldItemToPool(_itemObject);
    }
    
    private void NormalizeVisualScale(GameObject obj, float targetSize)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return;

        Bounds combinedBounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (var r in renderers)
            combinedBounds.Encapsulate(r.bounds);

        Vector3 s = combinedBounds.size;

        // 기하평균(부피 기준 대표 길이): (x * y * z)^(1/3)
        const float EPS = 1e-6f;
        float volume = Mathf.Max(EPS, s.x * s.y * s.z);
        float geoMean = Mathf.Pow(volume, 1f / 3f);

        if (geoMean <= EPS) return;

        float scaleFactor = targetSize / geoMean;
        obj.transform.localScale *= scaleFactor;
    }
}