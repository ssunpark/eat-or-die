public class SkillTrigger_HungerThreshold : ISkillTrigger<ISkillPayload>
{
    private readonly float _percent;
    private readonly bool _isAbove;

    public SkillTrigger_HungerThreshold(float percent, bool isAbove)
    {
        _percent = percent;
        _isAbove = isAbove;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        var result = _isAbove ? context.CurrentHunger / context.MaxHunger >= _percent : 
            context.CurrentHunger / context.MaxHunger <= _percent;
        return result;
    }
}