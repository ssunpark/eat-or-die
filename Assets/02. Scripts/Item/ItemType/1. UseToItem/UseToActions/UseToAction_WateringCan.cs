using UnityEngine;

public class UseToAction_WateringCan : IUseToAction
{
    public void UseTool(GameObject target)
    {
        if (target.TryGetComponent(out FarmingGround farmingGround))
        {
            // 물 주기
            farmingGround.Water();
        }
    }
}