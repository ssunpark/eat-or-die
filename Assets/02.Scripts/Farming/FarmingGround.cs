using Fusion;
using UnityEngine;

public class FarmingGround : NetworkBehaviour
{
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
        _plowedGround.SetActive(State == EFarmingGroundState.Plowed);

        if (State == EFarmingGroundState.Watered)
        {
            _baseGround.SetActive(false);
            _plowedGround.SetActive(true);
            _plowedGroundRenderer.material = _waterMaterial;
            _plowedSubGroundRenderer.material = _waterMaterial;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Plow()
    {
        if (State != EFarmingGroundState.None)
        {
            return;
        }
        
        // 밭 갈기
        if (Runner.IsServer)
        {
            State = EFarmingGroundState.Plowed;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Water()
    {
        if (State != EFarmingGroundState.Plowed)
        {
            return;
        }
        
        if (Runner.IsServer)
        {
            State = EFarmingGroundState.Watered;
        }
    }
}