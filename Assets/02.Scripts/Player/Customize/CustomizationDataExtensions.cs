using System;
using System.Collections.Generic;
using System.Reflection;

public static class CustomizationDataExtensions
{
    public static IEnumerable<(string key, short value)> AsEnumerable(this CustomizationData data)
    {
        yield return ("Axe", data.Axe);
        yield return ("Bag", data.Bag);
        yield return ("Bottom", data.Bottom);
        yield return ("Bracelet", data.Bracelet);
        yield return ("Earring", data.Earring);
        yield return ("Eye", data.Eye);
        yield return ("Eyebrow", data.Eyebrow);
        yield return ("Eyewear", data.Eyewear);
        yield return ("Glove", data.Glove);
        yield return ("Hair", data.Hair);
        yield return ("HairAcc", data.HairAcc);
        yield return ("HandAcc", data.HandAcc);
        yield return ("Headgear", data.Headgear);
        yield return ("Lips", data.Lips);
        yield return ("Mask", data.Mask);
        yield return ("Mustache", data.Mustache);
        yield return ("Shield", data.Shield);
        yield return ("Shoes", data.Shoes);
        yield return ("Spear", data.Spear);
        yield return ("Sword", data.Sword);
        yield return ("Top", data.Top);
        yield return ("Watch", data.Watch);
    }
}


public static class CustomizationDataMapper
{
    // Dictionary<string, int> → CustomizationData
    public static CustomizationData FromDictionary(Dictionary<ECustomizationPart, int> selections)
    {
        CustomizationData data = new();
        foreach (var field in typeof(CustomizationData).GetFields())
        {
            if (Enum.TryParse<ECustomizationPart>(field.Name, out var part) &&
                selections.TryGetValue(part, out int value))
            {
                field.SetValueDirect(__makeref(data), (short)value);
            }
        }
        return data;
    }

    public static Dictionary<ECustomizationPart, int> ToDictionary(CustomizationData data)
    {
        var dict = new Dictionary<ECustomizationPart, int>();
        foreach (var field in typeof(CustomizationData).GetFields())
        {
            if (Enum.TryParse<ECustomizationPart>(field.Name, out var part))
            {
                dict[part] = (short)field.GetValue(data);
            }
        }
        return dict;
    }

}
