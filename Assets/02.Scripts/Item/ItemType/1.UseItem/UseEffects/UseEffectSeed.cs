using UnityEngine;

public class UseEffectSeed : IUseEffect
{
    private readonly int _seedID;

    public UseEffectSeed(int seedID)
    {
        _seedID = seedID;
    }
    
    public void Use(GameObject target)
    {
        if (target.TryGetComponent(out SeedGround seedGround))
        {
            // 씨앗 심기
            seedGround.Plant(_seedID);
        }
    }
}