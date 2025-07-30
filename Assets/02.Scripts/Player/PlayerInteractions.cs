using Fusion;
using UnityEngine;

// 플레이어 상호작용
public class PlayerInteractions : MonoBehaviour
{

    public LayerMask InteractionLayer;
    public string TagName;
    public int HoldItemID;
    public string ItemName;
    public PlayerController _controller; // 플레이어 컨트롤러 참조

    public GameObject InteractionObject;       // 태그 일치용
    public GameObject UntaggedObject;          // 태그 없는 객체용

    private GameObject _targetObject;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        InteractionLayer = LayerMask.GetMask("Interactable");
        TagName = "Untagged";
    }

    //상호작용 (예: 작물 수확 등)
    public bool TryInteract(out IInteractable interactable)
    {
        SearchInteractables();
        if (UntaggedObject?.TryGetComponent(out IInteractable plant) ?? false)
        {
            interactable = plant; // 태그 없는 객체 대상
            _targetObject = UntaggedObject;
            return true;
        }
        interactable = null;
        return false;
    }

    //아이템 사용 (예: 괭이질, 물주기 등)
    public bool TryUseItem(out IUsable usable)
    {
        SearchInteractables();
        var handitem = ItemManager.Instance.GetItem(HoldItemID);
        if (handitem is IUsable useToItem)
        {
            usable = useToItem;
            _targetObject = InteractionObject;
            Debug.Log($"아이템 사용: {useToItem.InteractionTag} on {_targetObject?.name ?? "null"}");
            return true;
        }
        usable = null;
        return false;
    }

    public void UseOrInteract(IUsable usable = null, IInteractable interactable = null)
    {
        if (usable == null && _targetObject == null)
        {
            Debug.LogWarning("사용할 아이템이나 상호작용 대상이 없습니다.");
            return;
        }
        if (usable!=null)
        {
            if(_targetObject != null)
            {
                Debug.Log($"아이템 사용: {usable.InteractionTag} on {_targetObject.name}");
                usable.Use(_targetObject); // 태그 있는 객체 대상
                return;
            }
            else
            {
                Debug.LogWarning("사용할 대상이 없습니다.");
                return;
            }
        }
        else if (interactable != null)
        {
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning("상호작용할 대상이 없습니다.");
        }

    }

    public void SearchInteractables()
    {
        if (!_controller.HasInputAuthority)
            return;

        var colliderArray = Physics.OverlapSphere(transform.position, 10, InteractionLayer);

        Collider taggedClosest = null;
        float taggedClosestDist = float.MaxValue;

        Collider untaggedClosest = null;
        float untaggedClosestDist = float.MaxValue;

        foreach (var collider in colliderArray)
        {
            float dist = Vector3.Distance(transform.position, collider.transform.position);

            // 1. 태그 일치하는 객체 찾기
            if (!string.IsNullOrEmpty(TagName) && collider.CompareTag(TagName))
            {
                if (dist < taggedClosestDist)
                {
                    taggedClosestDist = dist;
                    taggedClosest = collider;
                }
            }

            // 2. 태그 없는 객체("Untagged") 찾기
            if (collider.CompareTag("Untagged"))
            {
                if (dist < untaggedClosestDist)
                {
                    untaggedClosestDist = dist;
                    untaggedClosest = collider;
                }
            }
        }

        InteractionObject = taggedClosest?.gameObject;
        UntaggedObject = untaggedClosest?.gameObject;
    }

    public void OnEquipped(int holdItemID)
    {
        HoldItemID = holdItemID;
        var item = ItemManager.Instance.GetItem(HoldItemID);
        ItemName = item.ItemData.Name;
        
        UsableItemInfo usableItem = item as UsableItemInfo;
        TagName = usableItem?.InteractionTag ?? "Untagged";
    }

    public void OnEquipped(AItemInfo itemInfo)
    {
        HoldItemID = itemInfo?.ItemData.ID ?? 0;
        ItemName = itemInfo?.ItemData.Name ?? "빈손";
        TagName = itemInfo is IUsable usableItem ? usableItem.InteractionTag : "Untagged";
    }

    public void OnUnequipped()
    {
        HoldItemID = 0;
        ItemName = "빈손";
        TagName = string.Empty;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위를 파란색 구체로 그립니다.
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
#endif
}