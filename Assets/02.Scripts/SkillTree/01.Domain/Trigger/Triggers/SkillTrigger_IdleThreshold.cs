public class SkillTrigger_IdleThreshold : ISkillTrigger<ISkillPayload>
{
    private readonly float _thresholdSecond;

    public SkillTrigger_IdleThreshold(float thresholdSecond)
    {
        _thresholdSecond = thresholdSecond;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        var playerFsm = context.Player.PlayerFSM;
        if (context.IsIdle && playerFsm.StateMachine.StateTime >= _thresholdSecond)
        {
            return true;
        }
        return false;
    }
}