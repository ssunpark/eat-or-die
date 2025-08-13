public class SkillTrigger_StateThreshold : ISkillTrigger<ISkillPayload>
{
    private readonly float _thresholdSecond;
    private readonly EPlayerState _compareState;

    private float _prevTime;

    public SkillTrigger_StateThreshold(float thresholdSecond, EPlayerState compareState)
    {
        _thresholdSecond = thresholdSecond;
        _compareState = compareState;
        _prevTime = 0f;
    }

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        var playerFsm = context.Player.PlayerFSM;

        if (playerFsm.StateMachine.StateTime < _thresholdSecond)
        {
            _prevTime = 0f;
        }
        
        if (context.CurrentState == _compareState && playerFsm.StateMachine.StateTime - _prevTime >= _thresholdSecond)
        {
            _prevTime = playerFsm.StateMachine.StateTime;
            return true;
        }
        return false;
    }
}