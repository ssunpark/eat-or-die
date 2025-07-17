using System.Collections.Generic;
public class FoodItemFactory
{
    public AItem CreateFoodItem(FoodCSVData data)
    {
        FoodItemData itemData = new FoodItemData(data);
        List<IUseItemEffect> effects = new List<IUseItemEffect>();

        // 예시: 효과 개수만큼 등록
        if (data.EffectCount >= 1)
            effects.Add(new UseItemEffect_Hungry(data.EffectValue1));

        if (data.EffectCount >= 2)
            effects.Add(new UseItemEffect_Hungry(data.EffectValue2));

        if (data.EffectCount >= 3)
            effects.Add(new UseItemEffect_Hungry(data.EffectValue3));

        return new UseItem(itemData, effects);
    }
}
