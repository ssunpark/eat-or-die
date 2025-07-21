using System;
using Fusion;
using UnityEngine;

public class FarmingGround : NetworkBehaviour
{
    [Networked]
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
        _baseGround.SetActive(State == EFarmingGroundState.None);
        _plowedGround.SetActive(State != EFarmingGroundState.None);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
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
        
        _baseGround.SetActive(false);
        _plowedGround.SetActive(true);
    }

    public void Water()
    {
        if (State != EFarmingGroundState.Plowed)
        {
            return;
        }
        State = EFarmingGroundState.Watered;
        // 머티리얼 변경
        _plowedGroundRenderer.material = _waterMaterial;
        _plowedSubGroundRenderer.material = _waterMaterial;
    }
}