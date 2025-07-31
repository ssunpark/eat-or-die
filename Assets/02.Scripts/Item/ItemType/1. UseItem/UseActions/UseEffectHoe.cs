using UnityEngine;

public class UseEffectHoe : IUseEffect
{
    public void Use(GameObject target)
    {
        if (target.TryGetComponent(out FarmingGround farmingGround))
        {
            // 밭 갈기
            farmingGround.RPC_Plow();
        }
    }
}