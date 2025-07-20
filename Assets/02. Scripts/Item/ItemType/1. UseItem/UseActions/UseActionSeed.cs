using UnityEngine;

public class UseActionSeed : IUseAction
{
    private readonly int _seedID;

    public UseActionSeed(int seedID)
    {
        _seedID = seedID;
    }
    
    public void UseTool(GameObject target)
    {
        if (target.TryGetComponent(out SeedGround seedGround))
        {
            // 씨앗 심기
            seedGround.Plant(_seedID);
        }
    }
}