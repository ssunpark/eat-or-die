using System;
using Fusion;
using UnityEngine;

public class FarmingInteractionTest : NetworkBehaviour
{
    public LayerMask InteractionLayer;
    public string TagName;

    public GameObject InteractionObject;

    private void Start()
    {
        TagName = "FarmingGround";
    }

    private void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Hoe());
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
            }
            InteractionObject = null;
            // 테스트 태그 바꾸기 (아이템에 상호작용해야하는 태그를 분리할 예정)
            TagName = "PlantGround";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Seed(100005));
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_WateringCan());
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
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