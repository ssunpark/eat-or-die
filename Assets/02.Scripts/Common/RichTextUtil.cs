using System.Text.RegularExpressions;
using UnityEngine;

public static class RichTextUtil
{
    private static readonly Regex ColorOpenTag = new(@"<color=([^>]+)>", RegexOptions.IgnoreCase);
    private static readonly Regex ColorCloseTag = new(@"</color>", RegexOptions.IgnoreCase);

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
}