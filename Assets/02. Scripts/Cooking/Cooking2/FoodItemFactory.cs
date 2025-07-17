using System.Collections.Generic;
public class FoodItemFactory
{
    public AItem CreateFoodItem(FoodCSVData data)
    {
        FoodItemData foodData = new FoodItemData(data);
        return new FoodItem(foodData);
    }
}
