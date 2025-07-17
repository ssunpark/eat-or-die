using System.Collections.Generic;
public class FoodItemData : ItemData
{
    public bool Eatable { get; set; }
    public int HungerRestore { get; set; }
    
    public string Ingredient1ID { get; set; }
    public string Ingredient2ID { get; set; }
    
    public int EffectCount { get; set; }

    public List<FoodEffectData> Effects { get; private set; } = new List<FoodEffectData>();
    public FoodItemData(FoodCSVData csvData) : base(csvData.ID, csvData.Name, csvData.Description, csvData.MaxStack, csvData.IconPath)
    {
        Eatable = csvData.Eatable;
        HungerRestore = csvData.HungerRestore;
        Ingredient1ID = csvData.Ingredient1ID;
        Ingredient2ID = csvData.Ingredient2ID;
        EffectCount = csvData.EffectCount;
        
        if (!string.IsNullOrEmpty(csvData.EffectType1))
        {
            Effects.Add(new FoodEffectData(csvData.EffectType1, csvData.EffectValue1, csvData.Duration1));
        }
        if (!string.IsNullOrEmpty(csvData.EffectType2))
        {
            Effects.Add(new FoodEffectData(csvData.EffectType2, csvData.EffectValue2, csvData.Duration2));
        }
        if (!string.IsNullOrEmpty(csvData.EffectType3))
        {
            Effects.Add(new FoodEffectData(csvData.EffectType3, csvData.EffectValue3, csvData.Duration3));
        }
    }

    public class FoodEffectData
    {
        public string EffectType { get; set; }
        public int EffectValue { get; set; }
        public float Duration { get; set; }

        public FoodEffectData(string effectType, int effectValue, float duration)
        {
            EffectType = effectType;
            EffectValue = effectValue;
            Duration = duration;
        }
    }

}
