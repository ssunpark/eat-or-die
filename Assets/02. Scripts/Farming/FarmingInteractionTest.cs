using System;
using Fusion;
using UnityEngine;

public class FarmingInteractionTest : NetworkBehaviour
{
    public LayerMask InteractionLayer;
    public string TagName;
    public int HoldItemID;
    public string ItemName;

    public GameObject InteractionObject;

    private void Start()
    {
        TagName = "Untagged";
    }

    private void Update()
    {
        if (!GetComponent<NetworkObject>().HasInputAuthority)
        {
            return;
        }
        
        var colliderArray = Physics.OverlapSphere(transform.position, 10, InteractionLayer);

        Collider closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliderArray)
        {
            if (!collider.CompareTag(TagName))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }

        InteractionObject = closestCollider?.gameObject;

        if (Input.GetMouseButtonDown(1))
        {
            var Handitem = ItemManager.Instance.GetItem(HoldItemID);
            if (Handitem is IUsable useToItem)
            {
                useToItem.Use(InteractionObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 장착 대신 일단 클래스로 형변환
            if (ItemManager.Instance.GetItem(HoldItemID) is UsableItem useToItem)
            {
                TagName = useToItem._interactionTag;
                ItemName = useToItem.ItemData.Name;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // 인터페이스로 수정
            if (InteractionObject?.TryGetComponent(out PlantObject plant) ?? false)
            {
                plant.Interact();
            }
            else
            {
                TagName = "Untagged";
                InteractionObject = null;
            }
        }
    }
}