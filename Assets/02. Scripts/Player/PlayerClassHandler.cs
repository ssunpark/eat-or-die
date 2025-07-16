using Fusion;
using UnityEngine;
using System.Collections.Generic;

public enum CharacterClassType
{
    Warrior, Mage, Farmer, Chef
}

public class PlayerClassHandler : NetworkBehaviour
{
    [SerializeField] private CharacterClassType _classType;
    [SerializeField] private string _nickname;

    [Header("Customization Options")]
    [SerializeField] private string _axe;
    [SerializeField] private string _bag;
    [SerializeField] private string _bottom;
    [SerializeField] private string _bracelet;
    [SerializeField] private string _earring;
    [SerializeField] private string _eye;
    [SerializeField] private string _eyebrow;
    [SerializeField] private string _eyewear;
    [SerializeField] private string _glove;
    [SerializeField] private string _hair;
    [SerializeField] private string _hairAcc;
    [SerializeField] private string _handAcc;
    [SerializeField] private string _headgear;
    [SerializeField] private string _lips;
    [SerializeField] private string _mask;
    [SerializeField] private string _mustache;
    [SerializeField] private string _shield;
    [SerializeField] private string _shoes;
    [SerializeField] private string _spear;
    [SerializeField] private string _sword;
    [SerializeField] private string _top;
    [SerializeField] private string _watch;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority && !Object.HasInputAuthority)
            return;

        ApplyInitialStatsByClass();
        ApplyCustomization();
    }

    private void Awake()
    {
        ApplyInitialStatsByClass();
        ApplyCustomization();
    }

    private void ApplyInitialStatsByClass()
    {
        var stat = GetComponent<PlayerStat>();
        if (stat == null) return;

        switch (_classType)
        {
            case CharacterClassType.Warrior:
                stat.ApplyModifier(EStatType.Armor, new StatModifier(StatModifierType.Add, 10, this));
                stat.ApplyModifier(EStatType.Damage, new StatModifier(StatModifierType.Add, 5, this));
                break;
            case CharacterClassType.Mage:
                stat.ApplyModifier(EStatType.Damage, new StatModifier(StatModifierType.Add, 15, this));
                stat.ApplyModifier(EStatType.Armor, new StatModifier(StatModifierType.Add, -5, this));
                break;
            case CharacterClassType.Farmer:
                stat.ApplyModifier(EStatType.ConsumptionRate, new StatModifier(StatModifierType.Multiply, 0.8f, this));
                stat.ApplyModifier(EStatType.SprintingMultiplier, new StatModifier(StatModifierType.Add, 0.2f, this));
                break;
            case CharacterClassType.Chef:
                stat.ApplyModifier(EStatType.AttackSpeed, new StatModifier(StatModifierType.Multiply, 1.3f, this));
                stat.ApplyModifier(EStatType.Armor, new StatModifier(StatModifierType.Add, -3, this));
                break;
        }
    }

    private void ApplyCustomization()
    {
        Transform partRoot = transform.Find("Characters/Parts");
        if (partRoot == null)
        {
            Debug.LogWarning("Character parts root not found");
            return;
        }

        ActivatePart(partRoot, "Axe", _axe);
        ActivatePart(partRoot, "Bag", _bag);
        ActivatePart(partRoot, "Bottom", _bottom);
        ActivatePart(partRoot, "Bracelet", _bracelet);
        ActivatePart(partRoot, "Earring", _earring);
        ActivatePart(partRoot, "Eye", _eye);
        ActivatePart(partRoot, "Eyebrow", _eyebrow);
        ActivatePart(partRoot, "Eyewear", _eyewear);
        ActivatePart(partRoot, "Glove", _glove);
        ActivatePart(partRoot, "Hair", _hair);
        ActivatePart(partRoot, "HairAcc", _hairAcc);
        ActivatePart(partRoot, "HandAcc", _handAcc);
        ActivatePart(partRoot, "Headgear", _headgear);
        ActivatePart(partRoot, "lips", _lips);
        ActivatePart(partRoot, "Mask", _mask);
        ActivatePart(partRoot, "Mustache", _mustache);
        ActivatePart(partRoot, "Shield", _shield);
        ActivatePart(partRoot, "Shoes", _shoes);
        ActivatePart(partRoot, "Spear", _spear);
        ActivatePart(partRoot, "Sword", _sword);
        ActivatePart(partRoot, "Top", _top);
        ActivatePart(partRoot, "Watch", _watch);
    }

    private void ActivatePart(Transform root, string category, string selectedName)
    {
        Transform categoryTransform = root.Find(category);
        if (categoryTransform == null) return;

        foreach (Transform child in categoryTransform)
        {
            child.gameObject.SetActive(child.name == selectedName);
        }
    }

    public void SetCharacterInfo(
        CharacterClassType classType, string nickname,
        string axe, string bag, string bottom, string bracelet, string earring,
        string eye, string eyebrow, string eyewear, string glove,
        string hair, string hairAcc, string handAcc, string headgear,
        string lips, string mask, string mustache, string shield,
        string shoes, string spear, string sword, string top, string watch)
    {
        _classType = classType;
        _nickname = nickname;
        _axe = axe;
        _bag = bag;
        _bottom = bottom;
        _bracelet = bracelet;
        _earring = earring;
        _eye = eye;
        _eyebrow = eyebrow;
        _eyewear = eyewear;
        _glove = glove;
        _hair = hair;
        _hairAcc = hairAcc;
        _handAcc = handAcc;
        _headgear = headgear;
        _lips = lips;
        _mask = mask;
        _mustache = mustache;
        _shield = shield;
        _shoes = shoes;
        _spear = spear;
        _sword = sword;
        _top = top;
        _watch = watch;
    }
}
