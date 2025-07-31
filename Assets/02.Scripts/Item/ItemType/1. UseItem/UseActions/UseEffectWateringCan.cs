using UnityEngine;

public class UseEffectWateringCan : IUseEffect
{
    public void Use(GameObject target)
    {
        if (target.TryGetComponent(out FarmingGround farmingGround))
        {
            // 물 주기
            farmingGround.RPC_Water();
        }
    }
}