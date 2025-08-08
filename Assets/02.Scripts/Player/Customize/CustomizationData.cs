using Fusion;

public struct CustomizationData : INetworkStruct
{
    public short Axe, Bag, Bottom, Bracelet, Earring;
    public short Eye, Eyebrow, Eyewear, Glove, Hair;
    public short HairAcc, HandAcc, Headgear, Lips, Mask;
    public short Mustache, Shield, Shoes, Spear, Sword;
    public short Top, Watch;

    public CustomizationData(short axe = 0, short bag = 0, short bottom = 1, short bracelet = 0, short earring = 0,
                             short eye = 1, short eyebrow = 0, short eyewear = 0, short glove = 0, short hair = 1,
                             short hairAcc = 0, short handAcc = 0, short headgear = 0, short lips = 0, short mask = 0,
                             short mustache = 0, short shield = 0, short shoes = 0, short spear = 0, short sword = 0,
                             short top = 1, short watch = 0)
    {
        Axe = axe;
        Bag = bag;
        Bottom = bottom;
        Bracelet = bracelet;
        Earring = earring;
        Eye = eye;
        Eyebrow = eyebrow;
        Eyewear = eyewear;
        Glove = glove;
        Hair = hair;
        HairAcc = hairAcc;
        HandAcc = handAcc;
        Headgear = headgear;
        Lips = lips;
        Mask = mask;
        Mustache = mustache;
        Shield = shield;
        Shoes = shoes;
        Spear = spear;
        Sword = sword;
        Top = top;
        Watch = watch;
    }
}