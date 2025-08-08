using System.Text.RegularExpressions;

public static class RichTextUtil
{
    private static readonly Regex ColorOpenTag = new(@"<color=([^>]+)>", RegexOptions.IgnoreCase);
    private static readonly Regex ColorCloseTag = new(@"</color>", RegexOptions.IgnoreCase);

    // 모든 <color=...>을 새 색으로 변경
    public static string RecolorAll(string input, string newHexOrName)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return ColorOpenTag.Replace(input, $"<color={newHexOrName}>");
    }

    // 컬러 태그 제거 (색 없애기)
    public static string StripColors(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        string noOpen = ColorOpenTag.Replace(input, string.Empty);
        return ColorCloseTag.Replace(noOpen, string.Empty);
    }

    // n번째 컬러 구간만 변경하고 싶을 때 (0-based)
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
}