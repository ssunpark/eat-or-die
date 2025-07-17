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

    private GameObject _plantObject;
    private SeedData _seedData;

    private float _growthTime;

    private float _timer;

    public override void Spawned()
    {
        OnGrowthLevelChanged();
        _growthTime = _seedData.GrowthTime;
    }

    // 외형 적용
    private void ApplyVisual()
    {
        // 이전 레벨에 반환
        if (GrowthLevel > 1)
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

    // 단계 변화 감지
    private void OnGrowthLevelChanged()
    {
        ApplyVisual();
        FarmingManager.Instance.TryGetSeedData(PlantID, out _seedData);
    }
}