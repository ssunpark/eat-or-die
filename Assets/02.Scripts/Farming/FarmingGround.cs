using Fusion;
using UnityEngine;

public class FarmingGround : NetworkBehaviour
{
    private const string HOE_TAG = "Hoe";
    private const string WATER_TAG = "WateringCan";

    [Networked, OnChangedRender(nameof(OnStateChanged))]
    public EFarmingGroundState State { get; set; }

    [SerializeField]
    private GameObject _baseGround;

    [SerializeField]
    private GameObject _plowedGround;

    [SerializeField]
    private GameObject _plowedSubGround;

    [SerializeField]
    private Material _waterMaterial;

    private MeshRenderer _plowedGroundRenderer;
    private MeshRenderer _plowedSubGroundRenderer;

    private void Awake()
    {
        _plowedGroundRenderer = _plowedGround.GetComponent<MeshRenderer>();
        _plowedSubGroundRenderer = _plowedSubGround.GetComponent<MeshRenderer>();
    }

    public override void Spawned()
    {
        OnStateChanged();
    }

    public void OnStateChanged()
    {
        _baseGround.SetActive(State == EFarmingGroundState.None);
        _plowedGround.SetActive(State == EFarmingGroundState.Hoe);

        tag = State switch
        {
            EFarmingGroundState.None => HOE_TAG,
            EFarmingGroundState.Hoe => WATER_TAG,
            _ => "Untagged"
        };
        gameObject.tag = tag;

        if (State == EFarmingGroundState.WateringCan)
        {
            _baseGround.SetActive(false);
            _plowedGround.SetActive(true);
            _plowedGroundRenderer.material = _waterMaterial;
            _plowedSubGroundRenderer.material = _waterMaterial;
        }
    }

    public void Hoe()
    {
        RPC_Hoe();
    }

    public void WateringCan()
    {
        RPC_WateringCan();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Hoe()
    {
        if (State != EFarmingGroundState.None)
        {
            return;
        }

        // 밭 갈기
        if (Runner.IsServer)
        {
            State = EFarmingGroundState.Hoe;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_WateringCan()
    {
        if (State != EFarmingGroundState.Hoe)
        {
            return;
        }

        if (Runner.IsServer)
        {
            State = EFarmingGroundState.WateringCan;
        }
    }
}