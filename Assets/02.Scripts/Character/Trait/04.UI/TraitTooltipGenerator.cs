public static class TraitTooltipGenerator
{
    public static string GenerateTooltip(CharacterTraitData data)
    {
        string statName = data.AffectedStat.ToString(); // 혹은 한글로 변환 매핑

        string format = data.ModifierType switch
        {
            EStatModifierType.Add =>
                $"레벨당 {statName}이 <color=#FF5555>{data.ValuePerLevel}</color> 증가합니다.",

            EStatModifierType.Multiply =>
                $"레벨당 {statName}이 <color=#FF5555>{data.ValuePerLevel * 100}%</color> 증가합니다.",

            _ => "알 수 없는 트레잇 유형입니다."
        };

        return format;
    }
}
