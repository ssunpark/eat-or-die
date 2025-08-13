using Fusion;

public class SkillTrigger_EveryOneSecond : ISkillTrigger<ISkillPayload>
{
    private TickTimer _timer;
    private bool _hasTimer;

    public bool CanTrigger(ISkillPayload payload, SkillContext context)
    {
        if (!_hasTimer)
        {
            _timer = TickTimer.CreateFromSeconds(context.Player.Runner, 1f);
            _hasTimer = true;
            return true;
        }

        if (_timer.Expired(context.Player.Runner))
        {
            _timer = TickTimer.CreateFromSeconds(context.Player.Runner, 1f);
            return true;
        }

        return false;
    }
}