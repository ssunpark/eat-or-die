public class SkillTrigger_StateThreshold : ISkillTrigger<ISkillPayload>
{
    private readonly float _thresholdSecond;
    private readonly EPlayerState _compareState;

    public SkillTrigger_StateThreshold(float thresholdSecond, EPlayerState compareState)
    {
        _thresholdSecond = thresholdSecond;
        _compareState = compareState;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        var playerFsm = context.Player.PlayerFSM;
        if (context.CurrentState == _compareState && playerFsm.StateMachine.StateTime >= _thresholdSecond)
        {
            return true;
        }
        return false;
    }
}