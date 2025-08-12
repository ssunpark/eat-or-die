public class SkillTrigger_HungerAboveThreshold : ISkillTrigger<ISkillPayload>
{
    private float _percent;

    public float Percent { get => _percent; set => _percent = value; }

    public SkillTrigger_HungerAboveThreshold(float percent)
    {
        _percent = percent;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        if (payload is not OnEatPayload p)
            return false;

        return p.HungerRatio >= _percent;
    }
}