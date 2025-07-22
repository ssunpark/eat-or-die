using System.Linq;
using System.Text.RegularExpressions;

public static class EatEffectUtils
{
    private static readonly Regex PlaceholderWithPercentRegex = new(@"\{(\d+)(:[^\}]*)?\}%");

    public static string FormatSmart(string format, params float[] values)
    {
        // 복사해서 수정 가능한 배열 생성
        float[] adjusted = (float[])values.Clone();

        // {0}, {1}, ... 중 %가 붙은 것만 찾아서 *100
        foreach (Match match in PlaceholderWithPercentRegex.Matches(format))
        {
            if (int.TryParse(match.Groups[1].Value, out int index) &&
                index >= 0 && index < adjusted.Length)
            {
                adjusted[index] *= 100f;
            }
        }

        return string.Format(format, adjusted.Cast<object>().ToArray());
    }
}