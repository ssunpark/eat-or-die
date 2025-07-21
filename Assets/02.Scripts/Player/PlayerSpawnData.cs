using System;
using System.Collections.Generic;

[Serializable]
public struct PlayerSpawnData
{
    public ECharacterType classType;

    // Customization
    public string axe, bag, bottom, bracelet, earring;
    public string eye, eyebrow, eyewear, glove;
    public string hair, hairAcc, handAcc, headgear;
    public string lips, mask, mustache, shield;
    public string shoes, spear, sword, top, watch;

    // Stats
    public Dictionary<EStatType, float> baseStats;

    public string Nickname;

    public PlayerSpawnData(
        ECharacterType classType,
        string axe, string bag, string bottom, string bracelet, string earring,
        string eye, string eyebrow, string eyewear, string glove,
        string hair, string hairAcc, string handAcc, string headgear,
        string lips, string mask, string mustache, string shield,
        string shoes, string spear, string sword, string top, string watch,
        Dictionary<EStatType, float> baseStats, string nickname="박진혁")
    {
        this.classType = classType;
        this.axe = axe;
        this.bag = bag;
        this.bottom = bottom;
        this.bracelet = bracelet;
        this.earring = earring;
        this.eye = eye;
        this.eyebrow = eyebrow;
        this.eyewear = eyewear;
        this.glove = glove;
        this.hair = hair;
        this.hairAcc = hairAcc;
        this.handAcc = handAcc;
        this.headgear = headgear;
        this.lips = lips;
        this.mask = mask;
        this.mustache = mustache;
        this.shield = shield;
        this.shoes = shoes;
        this.spear = spear;
        this.sword = sword;
        this.top = top;
        this.watch = watch;
        this.baseStats = baseStats;
        Nickname = nickname;
    }
}