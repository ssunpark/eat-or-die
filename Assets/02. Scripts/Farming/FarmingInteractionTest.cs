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
    }

    [ContextMenu("Plow")]
    public void Plow()
    {
        var item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Hoe());
        item.UseOn(InteractionObject);
    }
    
    [ContextMenu("Water")]
    public void Water()
    {
        var item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_WateringCan());
        item.UseOn(InteractionObject);
    }
    
    [ContextMenu("Seed")]
    public void Seed()
    {
        var item = new UsableOnTargetItem(new ItemData(0, "Farming", "", 1, ""), new UseToAction_Seed(100005));
        item.UseOn(InteractionObject);
    }
}