using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class EatEffectManager
{
    private const string EFFECTTYPE_CSV_PATH = "/EatEffectTypeCSV/EatEffectType.csv";
    private Dictionary<EStatType, string> _descriptionTemplateDictionary;
    private Dictionary<EStatType, EStatModifierType> _statModifierTypeDictionary;
    
    private readonly Regex PlaceholderWithPercentRegex = new(@"\{(\d+)(:[^\}]*)?\}%");

    public EatEffectManager()
    {
        LoadDescriptions();
    }

    private void LoadDescriptions()
    {
        _descriptionTemplateDictionary = new Dictionary<EStatType, string>();
        _statModifierTypeDictionary = new Dictionary<EStatType, EStatModifierType>();
        var effectList =
            CSVLoader<EatEffectRawData>.LoadCSV($"{Application.streamingAssetsPath}{EFFECTTYPE_CSV_PATH}");
        foreach (var effect in effectList)
        {
            _descriptionTemplateDictionary.Add(effect.Type, effect.Description);
            _statModifierTypeDictionary.Add(effect.Type, effect.StatModifierType);
        }
    }

    public string GetDescription(EStatType type, params float[] values)
    {
        var description = _descriptionTemplateDictionary[type];
        return FormatSmart(description, values);
    }
    
    public EStatModifierType GetStatModifierType(EStatType type) => _statModifierTypeDictionary[type];

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