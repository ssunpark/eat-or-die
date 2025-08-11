public class HungerAboveTrigger : ISkillTrigger<OnEatPayload>
{
    private float _percent;
    
    public float Percent { get => _percent; set => _percent = value; }

    public HungerAboveTrigger(float percent)
    {
        _percent = percent;
    }
    
    public bool CanTrigger(OnEatPayload payload, SkillContext context)
    {
        return payload.HungerRatio > _percent;
    }
}