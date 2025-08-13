using System.Text.RegularExpressions;
using UnityEngine;

public static class RichTextUtil
{
    private static readonly Regex ColorOpenTag = new(@"<color=([^>]+)>", RegexOptions.IgnoreCase);
    private static readonly Regex ColorCloseTag = new(@"</color>", RegexOptions.IgnoreCase);

    // 플레이스홀더 패턴: {0}, {1:...}, {2}% 등
    private static readonly Regex PlaceholderAnyRegex = new(@"\{(\d+)(:[^\}]*)?\}%?", RegexOptions.Compiled);

    // --- HEX or color name ---
    public static string RecolorAll(string input, string newHexOrName)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return ColorOpenTag.Replace(input, $"<color={newHexOrName}>");
    }

    public static string StripColors(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        string noOpen = ColorOpenTag.Replace(input, string.Empty);
        return ColorCloseTag.Replace(noOpen, string.Empty);
    }

    public static string RecolorAt(string input, int index, string newHexOrName)
    {
        if (string.IsNullOrEmpty(input)) return input;
        int count = 0;
        return ColorOpenTag.Replace(input, m =>
        {
            if (count++ == index) return $"<color={newHexOrName}>"; 
            return m.Value;
        });
    }

    public static string RecolorAll(string input, Color color)
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        return RecolorAll(input, hex);
    }

    public static string RecolorAt(string input, int index, Color color)
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        return RecolorAt(input, index, hex);
    }

    /// <summary>
    /// 문자열 내 모든 {n} 플레이스홀더를 지정 색상으로 감쌉니다.
    /// </summary>
    public static string ColorizePlaceholders(string input, string colorHex)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(colorHex))
            return input;

        return PlaceholderAnyRegex.Replace(input, m =>
        {
            return $"<color={colorHex}>{m.Value}</color>";
        });
    }

    public static string ColorizePlaceholders(string input, Color color)
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        return ColorizePlaceholders(input, hex);
    }
}
