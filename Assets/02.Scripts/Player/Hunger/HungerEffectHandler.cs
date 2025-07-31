public class HungerEffectHandler
{
    private readonly StatManager _stat;
    private readonly ResourceManager _resource;
    private float _previousHungerRatio = 0f;
    private const string BuffSource = "Hunger_Full_Buff";
    private const string DebuffSource = "Hungry_Debuff";
    private const string CriticalSource = "Hungry_Critical";


    public HungerEffectHandler(ResourceManager resource, StatManager stat)
    {
        _resource = resource;
        _stat = stat;

        _resource.OnHungerChanged += EvaluateEffect;
    }
    

    private void EvaluateEffect(float current, float max)
    {
        float ratio = current / max;

        if (ratio >= 0.7f && _previousHungerRatio < 0.7f)
        {
            _previousHungerRatio = ratio;
            //배부름
            _stat.RemoveModifiersFrom(DebuffSource);
            _stat.RemoveModifiersFrom(CriticalSource);
            _stat.ApplyModifier(EStatType.MeleeDamage, new StatModifier(EStatModifierType.Multiply, 0.5f, BuffSource));
            _stat.ApplyModifier(EStatType.MoveSpeed, new StatModifier(EStatModifierType.Multiply, 0.3f, BuffSource));
            _stat.ApplyModifier(EStatType.SprintingMultiplier, new StatModifier(EStatModifierType.Multiply, 0.3f, BuffSource));
        }
        else if(ratio >= 0.3f && (_previousHungerRatio >= 0.7f||_previousHungerRatio < 0.3f))
        {
            _previousHungerRatio = ratio;
            //일반상태
            _stat.RemoveModifiersFrom(BuffSource);
            _stat.RemoveModifiersFrom(DebuffSource);
            _stat.RemoveModifiersFrom(CriticalSource);
        }
        else if (ratio < 0.3f && _previousHungerRatio >= 0.3f)
        {
            _previousHungerRatio = ratio;
            //배고픔
            _stat.RemoveModifiersFrom(BuffSource);
            _stat.RemoveModifiersFrom(CriticalSource);
            _stat.ApplyModifier(EStatType.MoveSpeed, new StatModifier(EStatModifierType.Multiply, -0.3f, DebuffSource));
        }
        else if (ratio < 0.1f && _previousHungerRatio >= 0.1f)
        {
            _previousHungerRatio = ratio;

            _stat.RemoveModifiersFrom(BuffSource);
            _stat.RemoveModifiersFrom(DebuffSource);
            _stat.ApplyModifier(EStatType.MoveSpeed,
                new StatModifier(EStatModifierType.Multiply, 1.5f, CriticalSource));
        }

    }
}
