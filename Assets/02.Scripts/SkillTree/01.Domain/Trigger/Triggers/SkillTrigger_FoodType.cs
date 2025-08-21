using UnityEngine;

public class SkillTrigger_FoodType : ISkillTrigger<ISkillPayload>
{
    private readonly bool _isHarvest;

    public SkillTrigger_FoodType(bool isHarvest)
    {
        _isHarvest = isHarvest;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        if (payload is OnEatPayload eatPayload)
        {
            return _isHarvest ? eatPayload.IsHarvest : !eatPayload.IsHarvest;
        }

        return false;
    }
}