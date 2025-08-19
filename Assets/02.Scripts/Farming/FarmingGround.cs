using EPOOutline;
using Fusion;
using UnityEngine;
using System.Collections.Generic;

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

    [Header("Outline")]
    [SerializeField]
    private OutlineController _outlineController;
    [SerializeField]
    private Outlinable _baseGroundOutlinable;
    [SerializeField]
    private Outlinable _plowedGroundOutlinable;

    private MeshRenderer _plowedGroundRenderer;
    private MeshRenderer _plowedSubGroundRenderer;

    [Header("Layer")]
    [SerializeField]
    private LayerMask InteractableLayerMask = default;
    [SerializeField]
    private LayerMask NoooooLayerMask = default;

    [Header("Name")]
    [SerializeField] private UI_NameTag _nameTag;


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
            _outlineController.InactiveOutline();
            _plowedGroundRenderer.material = _waterMaterial;
            _plowedSubGroundRenderer.material = _waterMaterial;
        }

        _outlineController.OutlineObject = State switch
        {
            EFarmingGroundState.None => _baseGroundOutlinable,
            EFarmingGroundState.Hoe => _plowedGroundOutlinable,
            _ => null
        };

        gameObject.layer = State switch
        {
            EFarmingGroundState.None => Mathf.RoundToInt(Mathf.Log(InteractableLayerMask.value, 2)),
            EFarmingGroundState.Hoe => Mathf.RoundToInt(Mathf.Log(InteractableLayerMask.value, 2)),
            EFarmingGroundState.WateringCan => Mathf.RoundToInt(Mathf.Log(NoooooLayerMask.value, 2)),
            _ => Mathf.RoundToInt(Mathf.Log(NoooooLayerMask.value, 2))
        };

        _nameTag.ObjName = State switch
        {
            EFarmingGroundState.None => "땅",
            EFarmingGroundState.Hoe => "밭(메마름)",
            EFarmingGroundState.WateringCan => "촉촉한 밭",
            _ => "밭"
        };
        _nameTag.ActName = State switch
        {
            EFarmingGroundState.None => "갈기",
            EFarmingGroundState.Hoe => "물주기",
            EFarmingGroundState.WateringCan => "씨뿌리기",
            _ => ""
        };
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