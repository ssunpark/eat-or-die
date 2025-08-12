using System;
using System.Globalization;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

public class CsvEnumArrayConverter<TEnum> : DefaultTypeConverter where TEnum : struct, Enum
{
    private readonly char[] _seps;

    public CsvEnumArrayConverter(params char[] separators)
    {
        _seps = separators is { Length: > 0 } ? separators : new[] { ',', '|' };
    }

    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<TEnum>();

        var tokens = text.Split(_seps, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim());

        var list = tokens.Select(token =>
        {
            // 숫자도 허용 (예: "0|2|5")
            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var num))
                return (TEnum)Enum.ToObject(typeof(TEnum), num);

            if (Enum.TryParse<TEnum>(token, ignoreCase: true, out var val))
                return val;

            // 알 수 없는 값은 예외로 처리(원하면 무시하도록 바꿔도 됨)
            throw new TypeConverterException(this, memberMapData, token, row.Context, 
                $"Unknown {typeof(TEnum).Name} value: '{token}'");
        }).ToArray();

        return list;
    }

    public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        if (value is TEnum[] arr)
            return string.Join("|", arr.Select(v => v.ToString()));
        return base.ConvertToString(value, row, memberMapData);
    }
}

public class ESkillTriggerTypeArrayConverter : CsvEnumArrayConverter<ESkillTriggerType>
{
    public ESkillTriggerTypeArrayConverter() : base('|', ',') {}
}