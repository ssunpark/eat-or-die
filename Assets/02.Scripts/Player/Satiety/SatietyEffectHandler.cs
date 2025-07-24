public class SatietyEffectHandler
{
    private readonly StatManager _stat;
    private readonly ResourceManager _resource;

    private const string BuffSource = "SatietyBuff";
    private const string DebuffSource = "SatietyDebuff";
    private const string CriticalSource = "SatietyCritical";


    public SatietyEffectHandler(ResourceManager resource, StatManager stat)
    {
        _resource = resource;
        _stat = stat;

        _resource.OnSatietyChanged += EvaluateEffect;
    }

    private void EvaluateEffect(float current, float max)
    {
        float ratio = current / max;

        // 초기화
        _stat.RemoveModifiersFrom(BuffSource);
        _stat.RemoveModifiersFrom(DebuffSource);
        _stat.RemoveModifiersFrom(CriticalSource);

        if (ratio >= 0.7f)
        {
            _stat.ApplyModifier(EStatType.Damage, new StatModifier(EStatModifierType.Percentage, 0.5f, BuffSource));
            _stat.ApplyModifier(EStatType.MoveSpeed, new StatModifier(EStatModifierType.Multiply, 2f, CriticalSource));
            _stat.ApplyModifier(EStatType.SprintingMultiplier, new StatModifier(EStatModifierType.Percentage, 3f, CriticalSource));
        }
        else if (ratio <= 0.1f)
        {
            //광폭화

        }
        else if (ratio <= 0.3f)
        {
            _stat.ApplyModifier(EStatType.MoveSpeed, new StatModifier(EStatModifierType.Percentage, -0.3f, DebuffSource));
        }
    }
}
