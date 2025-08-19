using DarkTonic.MasterAudio;
using EPOOutline;
using Fusion;
using Redcode.Pools;
using UnityEngine;

public class PlantObject : NetworkBehaviour, IInteractable
{
    private const int ROTTEN_CROP_ID = 200012;
    private const string PARTICLE_KEY = "Farming_Alarm";
    // 작물의 성장과 결과물을 관리
    // 외형은 자식 오브젝트 생성해서 관리
    [Networked]
    public int PlantID { get; set; }
    [Networked, OnChangedRender(nameof(OnGrowthLevelChanged))]
    public int GrowthLevel { get; set; }

    private GameObject _plantObject;
    private SeedData _seedData;
    [SerializeField] private OutlineController _outlineController;
    [SerializeField] private LayerMask InteractableLayer;
    [SerializeField] private LayerMask NotInteractableLayer;

    private float _growthTime;

    private float _timer;

    private FarmingGround _farmingGround;
    private SeedGround _seedGround;
    private ParticleSystem _particleSystem;

    public bool IsImmediate => false;

    public float InteractionDistanceOffset => 0.2f;

    public override void Spawned()
    {
        _farmingGround = GetComponentInParent<FarmingGround>();
        _seedGround = GetComponentInParent<SeedGround>();
        OnGrowthLevelChanged();
        _growthTime = _seedData.GrowthTime;
    }

    // 외형 적용
    private void ApplyVisual()
    {
        // 이전 레벨에 반환
        if (GrowthLevel > 1 && _plantObject != null)
        {
            FarmingManager.Instance.ReturnPlant(new PlantPoolKey(PlantID, GrowthLevel - 1), _plantObject);
        }

        // 자식에 생성
        _plantObject = FarmingManager.Instance.GetPlant(new PlantPoolKey(PlantID, GrowthLevel));
        _plantObject.transform.SetParent(transform);
        _plantObject.transform.localPosition = Vector3.zero;
        float randomRotate = Random.Range(0f, 360f);
        _plantObject.transform.Rotate(0f, randomRotate, 0f, Space.Self);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer || _farmingGround.State != EFarmingGroundState.WateringCan)
        {
            return;
        }

        if (GrowthLevel >= _seedData.MaxGrowthLevel)
        {
            GrowthLevel = _seedData.MaxGrowthLevel;
            return;
        }

        _timer += Runner.DeltaTime * FarmingManager.Instance.GrowthTimeScale;
        if (_timer >= _growthTime)
        {
            _timer = 0;
            GrowthLevel += 1;
            _growthTime = GrowthLevel == _seedData.MaxGrowthLevel - 1 ? _seedData.DriedTime : _seedData.GrowthTime;
        }
    }

    public void Interact()
    {
        if (GrowthLevel < _seedData.MaxGrowthLevel - 1)
        {
            return;
        }

        // 상호 작용
        // 작물 생성
        if (GrowthLevel == _seedData.MaxGrowthLevel - 1)
        {
            // 작물 수확
            ItemProxySpawner.Instance.RPC_CreateItemObject(_seedData.HarvestItemID, 1, 1, transform.position,
                Quaternion.identity);
            MasterAudio.FireCustomEvent("PlantPop", transform);
        }
        else if (GrowthLevel >= _seedData.MaxGrowthLevel)
        {
            // 썩은 작물
            ItemProxySpawner.Instance.RPC_CreateItemObject(ROTTEN_CROP_ID, 1, 1, transform.position, Quaternion.identity);
            MasterAudio.FireCustomEvent("RottenPlantPop", transform);
        }
        
        // 풀 반환
        FarmingManager.Instance.ReturnPlant(new PlantPoolKey(PlantID, GrowthLevel), _plantObject);
        // 삭제
        RPC_Despawn();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Despawn()
    {
        if (!Runner.IsServer)
        {
            return;
        }
        
        _seedGround.IsPlanted = false;
        Runner.Despawn(Object);
    }

    // 단계 변화 감지
    private void OnGrowthLevelChanged()
    {
        ApplyVisual();
        _particleSystem?.Stop();
        FarmingManager.Instance.TryGetSeedData(PlantID, out _seedData);
        if(GrowthLevel<_seedData.MaxGrowthLevel - 1)
        {
            gameObject.layer = Mathf.RoundToInt(Mathf.Log(NotInteractableLayer.value, 2));
        }
        else
        {
            if (GrowthLevel == _seedData.MaxGrowthLevel - 1)
            {
                _particleSystem ??= ParticleManager.Instance.PlayByKeyLocalAsChild(PARTICLE_KEY, _plantObject.transform, Vector3.zero, Quaternion.identity);
                _particleSystem?.Play();
            }
            gameObject.layer = Mathf.RoundToInt(Mathf.Log(InteractableLayer.value, 2));
            _outlineController.OutlineObject = _plantObject.GetComponent<Outlinable>();
        }
    }
}