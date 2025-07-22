using System;
using Fusion;
using UnityEngine;

public class FarmingInteractionTest : NetworkBehaviour
{
    public LayerMask InteractionLayer;
    public string TagName;
    public int HoldItemID;
    public string ItemName;

    public GameObject InteractionObject;       // 태그 일치용
    public GameObject UntaggedObject;          // 태그 없는 객체용

    private void Start()
    {
        InteractionLayer = LayerMask.GetMask("Interactable");
        TagName = "Untagged";
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().HasInputAuthority)
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

        // 우클릭: 사용 (예: 괭이질, 물주기 등)
        if (Input.GetMouseButtonDown(1))
        {
            var handitem = ItemManager.Instance.GetItem(HoldItemID);
            if (handitem is IUsable useToItem)
            {
                useToItem.Use(InteractionObject); // 태그 있는 객체 대상
            }
        }
        // E키: 상호작용 (예: 작물 수확 등)
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (UntaggedObject?.TryGetComponent(out PlantObject plant) ?? false)
            {
                plant.Interact(); // 태그 없는 객체 대상
            }
        }
    }

    public void OnEquipped(int holdItemID, string interactionTag)
    {
        HoldItemID = holdItemID;
        ItemName = ItemManager.Instance.GetItem(HoldItemID).ItemData.Name;
        TagName = interactionTag;
    }

    public void OnUnequipped()
    {
        HoldItemID = 0;
        ItemName = "빈손";
        TagName = string.Empty;
    }
}
