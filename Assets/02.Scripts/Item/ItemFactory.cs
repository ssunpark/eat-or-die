using System.Collections.Generic;

public class ItemFactory
{
    // 음식 아이템 효과 factory
    private EatEffectFactory _eatEffectFactory = new();

    // 주어진 데이터에 맞게 아이템 생성 후 반환
    public EatItem CreateEatItem(EatItemRawData rawData)
    {
        var effectList = new List<IEatItemEffect>();

        var rawEffects = new (EEatItemEffectType type, float? value, float? duration)[]
        {
            (rawData.EffectType1, rawData.Value1, rawData.Duration1),
            (rawData.EffectType2, rawData.Value2, rawData.Duration2),
            (rawData.EffectType3, rawData.Value3, rawData.Duration3),
        };

        foreach (var (type, value, duration) in rawEffects)
        {
            if (type == EEatItemEffectType.None)
                continue;

            var effect = _eatEffectFactory.CreateEatItemEffect(type, value ?? 0f, duration ?? 0f);
            effectList.Add(effect);
        }

        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.Cookable,
            rawData.MaxQuantity, rawData.IconPath);
        return new EatItem(itemData, effectList);
    }

    // public EquipmentItem CreateEquipmentItem(EquipmentItemRawData rawData)
    // {
    //     var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, 1, "");
    //     return new EquipmentItem(itemData);
    // }
    //
    // public WeaponItem CreateWeaponItem(WeaponItemRawData rawData)
    // {
    //     var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, 1, "");
    //     return new WeaponItem(itemData, rawData.Type);
    // }

    public UsableItem CreateUsableItem(UsableItemRawData rawData)
    {
        var itemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, false, rawData.MaxQuantity,
            rawData.AddressablePath);
        IUseAction useAction = rawData.UseAction switch
        {
            EUseAction.Plow => new UseActionHoe(),
            EUseAction.Water => new UseActionWateringCan(),
            EUseAction.Plant => new UseActionSeed(rawData.ID),
        };
        return new UsableItem(itemData, rawData.InteractionTag, useAction);
    }
}