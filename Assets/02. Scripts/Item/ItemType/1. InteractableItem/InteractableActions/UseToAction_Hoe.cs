using UnityEngine;

public class UseToAction_Hoe : IUseToAction
{
    public void UseTool(GameObject target)
    {
        if (target.TryGetComponent(out FarmingGround farmingGround))
        {
            // 밭 갈기
            farmingGround.RPC_Plow();
        }
    }
}