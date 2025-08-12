public class SkillEffectAdapter<TPayload> : ISkillEffect where TPayload : ISkillPayload
{
    private readonly ISkillEffect<TPayload> _effect;
    public SkillEffectAdapter(ISkillEffect<TPayload> effect) => _effect = effect;

    public void Execute(ISkillPayload payload, SkillContext context)
    {
        if (payload is TPayload typedPayload)
            _effect.Execute(typedPayload, context);
    }
}