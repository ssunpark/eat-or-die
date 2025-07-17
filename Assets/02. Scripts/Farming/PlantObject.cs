using Fusion;
using UnityEngine;

public class PlantObject : NetworkBehaviour
{
    // 작물의 성장과 결과물을 관리
    // 외형은 자식 오브젝트 생성해서 관리
    [Networked]
    public int PlantID { get; set; }
    [Networked, OnChangedRender(nameof(OnGrowthLevelChanged))]
    public int GrowthLevel { get; set; }

    public override void Spawned()
    {
        // 기본 외형 적용
        ApplyVisual(FarmingManager.Instance.SeedDictionary[PlantID].PlantPrefabDictionary[GrowthLevel]);
    }

    // 외형 적용
    private void ApplyVisual(GameObject plantPrefab)
    {
        // 자식에 생성
        var plantObject = Instantiate(plantPrefab, transform);
    }

    // 단계 변화 감지
    private void OnGrowthLevelChanged()
    {
    }
}