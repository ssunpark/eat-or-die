using System.Collections.Generic;
using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    [SerializeField]
    private float _absorbRadius = 3f; // 흡수 범위
    [SerializeField]
    private LayerMask _itemLayerMask; // 아이템 레이어
    
    private void Update()
    {
        Pick();
    }

    private void Pick()
    {
        var items = DetectPickableItems();
        foreach (var item in items)
        {
            // 로컬 조건 체크 (인벤토리 매니저)
            // 서버 조건 체크 (인벤토리 매니저 RPC) => 아이템 스택의 오너가 설정됨
            // 아이템 흡수 연출 시작
            item.Pick();
            // 인벤토리 등록
        }
    }
    
    private List<IPickable> DetectPickableItems()
    {
        var pickableList = new List<IPickable>();
        var colliders = Physics.OverlapSphere(transform.position, _absorbRadius, _itemLayerMask);

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out IPickable pickable))
            {
                pickableList.Add(pickable);
            }
        }

        return pickableList;
    }
}