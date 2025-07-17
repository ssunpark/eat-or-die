using UnityEngine;

public class UseToAction_Seed : IUseToAction
{
    private readonly int _seedID;

    public UseToAction_Seed(int seedID)
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