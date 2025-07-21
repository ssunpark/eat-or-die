using UnityEngine;

public class UseActionWateringCan : IUseAction
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