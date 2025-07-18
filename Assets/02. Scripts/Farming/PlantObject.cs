using Fusion;
using Redcode.Pools;
using UnityEngine;

public class PlantObject : NetworkBehaviour
{
    // 작물의 성장과 결과물을 관리
    // 외형은 자식 오브젝트 생성해서 관리
    [Networked]
    public int PlantID { get; set; }
    [Networked, OnChangedRender(nameof(OnGrowthLevelChanged))]
    public int GrowthLevel { get; set; }
    [Networked]
    public NetworkId GroundNetworkId { get; set; } // 부모 객체 네트워크 ID
    [Networked, Capacity(24)]
    public string ParentPath { get; set; }

    private GameObject _plantObject;
    private SeedData _seedData;

    private float _growthTime;

    private float _timer;

    public override void Spawned()
    {
        if (Runner.TryFindObject(GroundNetworkId, out var netParentObj))
        {
            var target = netParentObj.GetComponent<FarmingGround>().PlowedGround.transform.Find(ParentPath);
            if (target != null)
            {
                transform.SetParent(target);
                transform.localPosition = Vector3.zero;
            }
        }
        
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
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer)
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
        // 상호 작용
        if (GrowthLevel == _seedData.MaxGrowthLevel - 1)
        {
            // 작물 수확
            ItemManager.Instance.RPC_CreateItemObject(_seedData.HarvestItemID, 1, transform.position, Quaternion.identity);
            FarmingManager.Instance.ReturnPlant(new PlantPoolKey(PlantID, GrowthLevel - 1), _plantObject);
        }
        else if (GrowthLevel >= _seedData.MaxGrowthLevel)
        {
            // 썩은 작물
        }
    }

    // 단계 변화 감지
    private void OnGrowthLevelChanged()
    {
        ApplyVisual();
        FarmingManager.Instance.TryGetSeedData(PlantID, out _seedData);
    }
}