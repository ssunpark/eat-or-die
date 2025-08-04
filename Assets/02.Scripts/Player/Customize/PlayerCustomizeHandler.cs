using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public enum ECustomizationPart
{
    Axe, Bag, Bottom, Bracelet, Earring,
    Eye, Eyebrow, Eyewear, Glove, Hair,
    HairAcc, HandAcc, Headgear, Lips, Mask,
    Mustache, Shield, Shoes, Spear, Sword,
    Top, Watch
}

public class PlayerCustomizeHandler : NetworkBehaviour
{
    [SerializeField] private ECharacterType _classType;
    [SerializeField] private string _nickname;
    [Networked,OnChangedRender(nameof(ApplyCustomization))] private CustomizationData _customData { get; set; }
    


    private Dictionary<ECustomizationPart, int> _customizeSelections = new();
    public override void Spawned()
    {

        if(!Object.HasInputAuthority)
            return;
        var holder = CustomizationDataHolder.Instance;
        Rpc_SetCharacterInfo(holder.ClassType, holder.Nickname, holder.CustomizationData);
    }

    private void Awake()
    {
        if (_customizeSelections.Count == 0)
        {
            foreach (ECustomizationPart part in Enum.GetValues(typeof(ECustomizationPart)))
            {
                _customizeSelections[part] = 0;
            }
        }
    }

    public void ApplyBtn()
    {
        if (Object.HasInputAuthority)
        {
            CustomizationData data = CustomizationDataMapper.FromDictionary(_customizeSelections);
            Rpc_SetCharacterInfo(_classType, _nickname, data);
        }
    }
    private void OnGUI()
    {
    //     if(!Object.HasInputAuthority)
    //         return;
    //     GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height));
    //     GUILayout.Label("Nickname:");
    //     _nickname = GUILayout.TextField(_nickname);
    //
    //     GUILayout.Space(10);
    //     GUILayout.Label("Class:");
    //     _classType = (ECharacterType)GUILayout.SelectionGrid(
    //     (int)_classType,
    //     Enum.GetNames(typeof(ECharacterType)),
    //     1
    // );
    //     GUILayout.Space(10);
    //
    //
    //     GUILayout.Label("Customization:");
    //     Dictionary<string, int> maxCounts = new()
    //     {
    //         ["Axe"] = 3,
    //         ["Bag"] = 18,
    //         ["Bottom"] = 55,
    //         ["Bracelet"] = 5,
    //         ["Earring"] = 20,
    //         ["Eye"] = 12,
    //         ["Eyebrow"] = 23,
    //         ["Eyewear"] = 18,
    //         ["Glove"] = 22,
    //         ["Hair"] = 28,
    //         ["HairAcc"] = 3,
    //         ["HandAcc"] = 10,
    //         ["Headgear"] = 63,
    //         ["Lips"] = 11,
    //         ["Mask"] = 5,
    //         ["Mustache"] = 29,
    //         ["Shield"] = 4,
    //         ["Shoes"] = 52,
    //         ["Spear"] = 3,
    //         ["Sword"] = 3,
    //         ["Top"] = 71,
    //         ["Watch"] = 5
    //     };
    //
    //     foreach (ECustomizationPart part in Enum.GetValues(typeof(ECustomizationPart)))
    //     {
    //         string name = part.ToString();
    //         int max = maxCounts[name];
    //         int current = _customizeSelections[part];
    //
    //         GUILayout.BeginHorizontal();
    //         GUILayout.Label(name, GUILayout.Width(100));
    //
    //         if (GUILayout.Button("-", GUILayout.Width(25)))
    //             _customizeSelections[part] = Mathf.Max(0, current - 1);
    //
    //         GUILayout.Label(current.ToString(), GUILayout.Width(30));
    //
    //         if (GUILayout.Button("+", GUILayout.Width(25)))
    //             _customizeSelections[part] = Mathf.Min(max, current + 1);
    //
    //         GUILayout.EndHorizontal();
    //     }
    //
    //     if (GUILayout.Button("Apply Customization"))
    //     {
    //         ApplyBtn();
    //     }
    //     GUILayout.EndArea();
    }

    private void ApplyCustomization()
    {
        var root = transform.Find("Characters/Parts");
        if (root == null) return;

        foreach (var (category, index) in _customData.AsEnumerable())
        {
            if (index <= 0) continue;
            ActivatePart(root, category, index);
        }
    }

    private void ActivatePart(Transform root, string category, short index)
    {
        string name = $"{category}_{index}";
        Transform categoryTransform = root.Find(category);
        if (categoryTransform == null) return;

        foreach (Transform child in categoryTransform)
            child.gameObject.SetActive(child.name == name);
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_SetCharacterInfo(ECharacterType classType, string nickname, CustomizationData data)
    {
        _classType = classType;
        _nickname = nickname;
        _customData = data;
        ApplyCustomization();
    }
}
