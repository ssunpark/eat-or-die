public class SkillTrigger_HungerBelowThreshold : ISkillTrigger<ISkillPayload>
{
    private float _percent;
    
    public float Percent { get => _percent; set => _percent = value; }

    public SkillTrigger_HungerBelowThreshold(float percent)
    {
        _percent = percent;
    }
    
    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        if (payload is not OnEatPayload p)
            return false;
        
        return p.HungerRatio > _percent;
    }
}