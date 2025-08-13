public class SkillTrigger_IsState : ISkillTrigger<ISkillPayload>
{
    private readonly EPlayerState _compareState;

    public SkillTrigger_IsState(EPlayerState compareState)
    {
        _compareState = compareState;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        return _compareState == context.CurrentState;
    }
}