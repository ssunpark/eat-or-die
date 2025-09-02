public static class TraitTooltipGenerator
{
    public static string GenerateTooltip(CharacterTraitData data)
    {
        if (data == null) return string.Empty;
        string statName = StatNameLocalization.Get(data.StatType);
        string marker = GetSubjectMarker(statName);

        string format = data.ModifierType switch
        {
            EStatModifierType.Add =>
            $"레벨당 {statName}{marker} <color=#FF5555>{data.ValuePerLevel}</color> 증가합니다.",

            EStatModifierType.Multiply =>
                $"레벨당 {statName}{marker} <color=#FF5555>{data.ValuePerLevel * 100}%</color> 증가합니다.",

            _ => "알 수 없는 트레잇 유형입니다."
        };

        return format;
    }

    private static string GetSubjectMarker(string word)
    {
        if (string.IsNullOrEmpty(word))
            return "이";

        char lastChar = word[word.Length - 1];
        if (lastChar < 0xAC00 || lastChar > 0xD7A3)
            return "이";

        int code = lastChar - 0xAC00;
        int jong = code % 28;
        return jong == 0 ? "가" : "이";
    }
}
