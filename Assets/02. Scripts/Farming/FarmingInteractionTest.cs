using System;
using Fusion;
using UnityEngine;

public class FarmingInteractionTest : NetworkBehaviour
{
    public LayerMask InteractionLayer;

    public GameObject InteractionObject;

    private void Update()
    {
        var colliderArray = Physics.OverlapSphere(transform.position, 10, InteractionLayer);

        Collider closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliderArray)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }

        InteractionObject = closestCollider?.gameObject;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Hoe());
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
            }
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Seed(100005));
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
            }
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            AItem item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_WateringCan());
            if (item is IUsableOnTarget usable)
            {
                usable.UseOn(InteractionObject);
            }
        }
    }
}