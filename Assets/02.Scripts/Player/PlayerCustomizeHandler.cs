using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


public class PlayerCustomizeHandler : NetworkBehaviour
{
    [SerializeField] private ECharacterType _classType;
    [SerializeField] private string _nickname;

    [Header("Customization Options")]
    [Networked,OnChangedRender(nameof(ApplyCustomization))] private string _axe { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _bag { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _bottom { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _bracelet { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _earring { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _eye { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _eyebrow{ get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _eyewear { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _glove { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _hair { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _hairAcc { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _handAcc { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _headgear { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _lips { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _mask { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _mustache { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _shield { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _shoes { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _spear { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _sword { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _top { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private string _watch { get; set; }


    private Dictionary<string, int> _customizeSelections = new();
    public override void Spawned()
    {
        ApplyCustomization();
    }

    private void Awake()
    {
        string[] categories = new string[] {
            "Axe", "Bag", "Bottom", "Bracelet", "Earring", "Eye", "Eyebrow", "Eyewear",
            "Glove", "Hair", "HairAcc", "HandAcc", "Headgear", "Lips", "Mask", "Mustache",
            "Shield", "Shoes", "Spear", "Sword", "Top", "Watch"
        };

        if(_customizeSelections.Count == 0)
        {
            _classType = ECharacterType.Warrior;
            foreach (var category in categories)
                _customizeSelections[category] = 0;
        }
    }

    public void ApplyBtn()
    {
        if (Object.HasInputAuthority)
        {
            string GetName(string part) => _customizeSelections[part] > 0 ? part + "_" + _customizeSelections[part] : "";

            //RPC 전달
            Rpc_SetCharacterInfo(
                _classType, _nickname,
                GetName("Axe"), GetName("Bag"), GetName("Bottom"), GetName("Bracelet"), GetName("Earring"),
                GetName("Eye"), GetName("Eyebrow"), GetName("Eyewear"), GetName("Glove"),
                GetName("Hair"), GetName("HairAcc"), GetName("HandAcc"), GetName("Headgear"),
                GetName("Lips"), GetName("Mask"), GetName("Mustache"), GetName("Shield"),
                GetName("Shoes"), GetName("Spear"), GetName("Sword"), GetName("Top"), GetName("Watch")
            );
        }
    }
    private void OnGUI()
    {
        if(!Object.HasInputAuthority)
            return;
        GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height));
        GUILayout.Label("Nickname:");
        _nickname = GUILayout.TextField(_nickname);

        GUILayout.Space(10);
        GUILayout.Label("Class:");
        _classType = (ECharacterType)GUILayout.SelectionGrid(
        (int)_classType,
        Enum.GetNames(typeof(ECharacterType)),
        1
    );
        GUILayout.Space(10);


        GUILayout.Label("Customization:");
        Dictionary<string, int> maxCounts = new()
        {
            ["Axe"] = 3,
            ["Bag"] = 18,
            ["Bottom"] = 55,
            ["Bracelet"] = 5,
            ["Earring"] = 20,
            ["Eye"] = 12,
            ["Eyebrow"] = 23,
            ["Eyewear"] = 18,
            ["Glove"] = 22,
            ["Hair"] = 28,
            ["HairAcc"] = 3,
            ["HandAcc"] = 10,
            ["Headgear"] = 63,
            ["Lips"] = 11,
            ["Mask"] = 5,
            ["Mustache"] = 29,
            ["Shield"] = 4,
            ["Shoes"] = 52,
            ["Spear"] = 3,
            ["Sword"] = 3,
            ["Top"] = 71,
            ["Watch"] = 5
        };

        foreach (var kvp in maxCounts)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(kvp.Key, GUILayout.Width(100));

            // 감소 버튼
            if (GUILayout.Button("-", GUILayout.Width(25)))
                _customizeSelections[kvp.Key] = Mathf.Max(0, _customizeSelections[kvp.Key] - 1);

            // 현재 값 표시
            GUILayout.Label(_customizeSelections[kvp.Key].ToString(), GUILayout.Width(30));

            // 증가 버튼
            if (GUILayout.Button("+", GUILayout.Width(25)))
                _customizeSelections[kvp.Key] = Mathf.Min(kvp.Value, _customizeSelections[kvp.Key] + 1);

            GUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Apply Customization"))
        {
            ApplyBtn();
        }
        GUILayout.EndArea();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetCharacterInfo(
    ECharacterType classType, string nickname,
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
        ActivatePart(partRoot, "Lips", _lips);
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
        ECharacterType classType, string nickname,
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
