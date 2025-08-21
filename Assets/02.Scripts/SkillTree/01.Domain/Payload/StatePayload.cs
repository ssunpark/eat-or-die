public class StatePayload : ISkillPayload
{
    public bool IsEnter { get; private set; } = true;

    public StatePayload(bool isEnter)
    {
        IsEnter = isEnter;
    }
}