using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using UnityEngine;

[Serializable]
public class ModifierDefinition
{
    [Name("TID")]
    public int TID;
    [Name("EStatType")]
    public EStatType StatType;
    [Name("EStatModifierType")]
    public EStatModifierType StatModifierType;
    [Name("Description")]
    public string Description;
    public float Value;
    public float DurationSec;
}


[Serializable]
public class FoodEffectRow // from FoodEffectCSV (설명 템플릿)
{
    [Name("EStatType")]
    public EStatType StatType { get; set; }

    [Name("EStatModifierType")]
    public EStatModifierType Op { get; set; }

    [Name("Description")]
    public string Description { get; set; }

    [Name("ExtraDescription")]
    public string ExtraDescription { get; set; }
}

[Serializable]
public class FoodRow // from FoodCSV (아이템별 효과)
{
    [Name("ID")] public int FoodId { get; set; }
    [Name("Name")] public string Name { get; set; }
    [Name("Description")] public string Description { get; set; }
    [Name("IsIngredient")] public bool IsIngredient { get; set; }
    [Name("HasDurability")] public bool HasDurability { get; set; }
    [Name("MaxStack")] public int MaxStack { get; set; }
    [Name("HungerRestore")] public float HungerRestore { get; set; }

    [Name("EEffectType1")] public EStatType? EffectStat1 { get; set; }
    [Name("EffectValue1")] public float? Value1 { get; set; }
    [Name("Duration1")] public float? Duration1 { get; set; }

    [Name("EEffectType2")] public EStatType? EffectStat2 { get; set; }
    [Name("EffectValue2")] public float? Value2 { get; set; }
    [Name("Duration2")] public float? Duration2 { get; set; }

    [Name("EEffectType3")] public EStatType? EffectStat3 { get; set; }
    [Name("EffectValue3")] public float? Value3 { get; set; }
    [Name("Duration3")] public float? Duration3 { get; set; }

    [Name("IconPath(Addressable Key)")] public string IconPath { get; set; }
    [Name("PrefabPath(Addressable)")] public string PrefabPath { get; set; }
    [Name("InteractionTag")] public string InteractionTag { get; set; }
}

