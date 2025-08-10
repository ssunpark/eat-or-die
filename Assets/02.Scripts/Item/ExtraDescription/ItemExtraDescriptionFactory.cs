using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemExtraDescriptionFactory
{
    private const string EXTRA_DESCRIPTION_CSV_PATH = "/ItemCSV/ItemExtraDescription.csv";
    private Dictionary<(EItemType, EStatType), string> _descriptionTemplateDictionary;
    
    private readonly Regex PlaceholderWithPercentRegex = new(@"\{(\d+)(:[^\}]*)?\}%");
    private readonly Regex PlaceholderAnyRegex = new(@"\{(\d+)(:[^\}]*)?\}%?");

    public ItemExtraDescriptionFactory()
    {
        LoadDescriptions();
    }

    private void LoadDescriptions()
    {
        _descriptionTemplateDictionary = new Dictionary<(EItemType, EStatType), string>();
        var descriptionList =
            CSVLoader<ItemExtraDescriptionRawData>.LoadCSV($"{Application.streamingAssetsPath}{EXTRA_DESCRIPTION_CSV_PATH}");
        foreach (var description in descriptionList)
        {
            _descriptionTemplateDictionary.Add((description.ItemType, description.StatType), description.Description);
        }
    }

    public string GetDescription(EItemType itemType, EStatType statType, string colorHex, params float[] values)
    {
        var template = _descriptionTemplateDictionary[(itemType, statType)];
        var result =  FormatSmart(template, values);
        var coloredTemplate = ApplySingleColorToPlaceholders(template, colorHex);

        return FormatSmart(coloredTemplate, values);
    }
    
    public string GetDescription(EItemType itemType, EStatType statType, Color color, params float[] values)
    {
        string hex = $"#{ColorUtility.ToHtmlStringRGB(color)}";
        return GetDescription(itemType, statType, hex, values);
    }

    public string FormatSmart(string format, params float[] values)
    {
        // Clone original for percentage handling
        float[] adjusted = (float[])values.Clone();

        // {0}% 같이 퍼센트 표시가 붙은 값은 *100 처리
        foreach (Match match in PlaceholderWithPercentRegex.Matches(format))
        {
            if (int.TryParse(match.Groups[1].Value, out int index) &&
                index >= 0 && index < adjusted.Length)
            {
                adjusted[index] *= 100f;
            }
        }

        // 객체 배열로 변환하면서 시간 형식 변환도 처리
        object[] finalValues = adjusted
            .Select((v, i) =>
            {
                // {1}이면 duration → 시간 변환 시도
                if (i == 1)
                    return FormatDuration(v);
                return (object)v;
            })
            .ToArray();

        return string.Format(format, finalValues);
    }
    
    private string ApplySingleColorToPlaceholders(string format, string colorHex)
    {
        if (string.IsNullOrEmpty(colorHex)) return format;
    
        return PlaceholderAnyRegex.Replace(format, m =>
        {
            // m.Value 전체가 "{n}" 또는 "{n}%" 형태라 퍼센트까지 같이 감싸짐
            return $"<color={colorHex}>{m.Value}</color>";
        });
    }

    private string FormatDuration(float seconds)
    {
        int totalSeconds = Mathf.RoundToInt(seconds);
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;

        if (minutes > 0 && secs > 0)
            return $"{minutes}분 {secs}초";
        if (minutes > 0)
            return $"{minutes}분";
        return $"{secs}초";
    }
}