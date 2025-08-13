using UnityEngine;

public class SkillTrigger_OnNChance : ISkillTrigger<ISkillPayload>
{
    private readonly float _nValue;

    public SkillTrigger_OnNChance(float nValue)
    {
        _nValue = nValue;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        float rand = Random.value;
        return rand < _nValue;
    }
}