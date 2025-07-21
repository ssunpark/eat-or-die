using UnityEngine;

public class UseActionHoe : IUseAction
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