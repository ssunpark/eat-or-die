using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
public enum ECustomizationPart
{
    Axe, Bag, Bottom, Bracelet, Earring,
    Eye, Eyebrow, Eyewear, Glove, Hair,
    HairAcc, HandAcc, Headgear, Lips, Mask,
    Mustache, Shield, Shoes, Spear, Sword,
    Top, Watch
}

public struct CustomizationSnapshot : INetworkStruct
{
    public ECharacterType ClassType;
    public NetworkString<_16> Nickname;
}

public class PlayerCustomizeHandler : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnClassTypeChanged))] private ECharacterType _classType { get; set; }
    public ECharacterType ClassType => _classType;

    [Networked]
    public CustomizationSnapshot NetCustomize { get; set; }

    [Networked, OnChangedRender(nameof(ApplyNickname))] public string Nickname { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private CustomizationData _customData { get; set; }

    [SerializeField] private TextMeshProUGUI _nicknameText;

    private CharacterDataInstaller _installer;
    private bool _installed;

    private Dictionary<ECustomizationPart, int> _customizeSelections = new();
    public override void Spawned()
    {
        _installer = GetComponent<CharacterDataInstaller>();

        InitAndSendCustomizationAsync().Forget();


        TryInstall();
    }

    private async UniTaskVoid InitAndSendCustomizationAsync()
    {
        // 파괴/씬전환 시 자동 취소
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.WaitUntil(
                () => PlayerInfoManager.Instance != null && PlayerInfoManager.Instance.Object != null,
                cancellationToken: token
            );
        if (Object.HasInputAuthority)
        {
            var holder = CustomizationDataHolder.Instance;
            var nickname = string.IsNullOrEmpty(holder?.Nickname)
                ? $"Test{UnityEngine.Random.Range(0, 999)}"
                : holder.Nickname;

            var snapshot = new CustomizationSnapshot
            {
                ClassType = holder != null ? holder.ClassType : ECharacterType.Warrior,
                Nickname = nickname
            };
            var data = holder != null ? holder.CustomizationData : default;

            // 이제 RPC 송신
            RPC_RequestApplyCustomization(snapshot, data);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_RequestApplyCustomization(CustomizationSnapshot snapshot, CustomizationData data, RpcInfo info = default)
    {

        _classType = snapshot.ClassType;
        Nickname = snapshot.Nickname.ToString();
        NetCustomize = snapshot;
        _customData = data;

        PlayerInfoManager.Instance.UpdateNickname(Object.InputAuthority, Nickname);
    }

    private void OnClassTypeChanged()
    {
        TryInstall();
        ApplyCustomization();
    }

    private void TryInstall()
    {
        if (_installed || _installer == null) return;

        _installer.Install(_classType);
        _installed = true;
    }

    private void ApplyNickname()
    {
        if (_nicknameText != null)
            _nicknameText.text = Nickname;
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
        _partsRoot = transform.Find("Characters/Parts");
    }

    private void ApplyCustomization()
    {
        var root = _partsRoot != null ? _partsRoot : transform.Find("Characters/Parts");
        if (root == null) return;

        foreach (var (category, index) in _customData.AsEnumerable())
        {
            if (index <= 0) continue;
            ActivatePart(root, category, index);
        }

        SetArmor(); 
        ApplyNickname();
    }
    private bool _didFirstRenderInit;

    public override void Render()
    {
        if (_didFirstRenderInit) return;

        ApplyCustomization();

        _didFirstRenderInit = true;
    }

    private void ActivatePart(Transform root, string category, short index)
    {
        string name = $"{category}_{index}";
        Transform categoryTransform = root.Find(category);
        if (categoryTransform == null) return;

        foreach (Transform child in categoryTransform)
            child.gameObject.SetActive(child.name == name);
    }

    private Transform _partsRoot;

    [Networked, OnChangedRender(nameof(OnEquipFlagsChanged))]
    public NetworkBool EquipedHelmet { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnEquipFlagsChanged))]
    public NetworkBool EquipedArmor { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnEquipFlagsChanged))]
    public NetworkBool EquipedLeggings { get; set; } = false;
    [Networked, OnChangedRender(nameof(OnEquipFlagsChanged))]
    public NetworkBool EquipedBoots { get; set; } = false;

    private void OnEquipFlagsChanged() => SetArmor();
    [SerializeField] private List<GameObject> _helmetPrefabs;
    [SerializeField] private List<GameObject> _armorPrefabs;
    [SerializeField] private List<GameObject> _leggingsPrefabs;
    [SerializeField] private List<GameObject> _bootsPrefabs;
    private GameObject _savedTop;
    private GameObject _savedBottom;
    private GameObject _currentArmor;
    private GameObject _currentLeggings;
    private GameObject GetActiveChild(string category)
    {
        var t = (_partsRoot ?? transform.Find("Characters/Parts"))?.Find(category);
        if (t == null) return null;

        for (int i = 0; i < t.childCount; i++)
        {
            var go = t.GetChild(i).gameObject;
            if (go.activeSelf) return go;
        }
        return null;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_EquipOrUnequipSomething(EArmorType type, bool equip)
    {
        switch (type)
        {
            case EArmorType.Helmet: EquipedHelmet = equip; break;
            case EArmorType.Chestplate: EquipedArmor = equip; break;
            case EArmorType.Leggings: EquipedLeggings = equip; break;
            case EArmorType.Boots: EquipedBoots = equip; break;
            default:
                Debug.LogWarning($"[PlayerCustomizeHandler] 알 수 없는 장비 타입: {type}");
                break;
        }
        SetArmor();
    }

    public void SetArmor()
    {
        int classIdx = (int)_classType;

        // --- Helmet ---
        if (_helmetPrefabs != null && classIdx < _helmetPrefabs.Count)
            _helmetPrefabs[classIdx].SetActive(EquipedHelmet);

        // --- Chestplate(상의) ---
        if (_armorPrefabs != null && classIdx < _armorPrefabs.Count)
        {
            if (EquipedArmor)
            {
                // 아직 저장 안 했다면 현재 Top을 저장
                if (_savedTop == null) _savedTop = GetActiveChild("Top");
                if (_savedTop != null) _savedTop.SetActive(false);

                _currentArmor = _armorPrefabs[classIdx];
                _currentArmor.SetActive(true);
            }
            else
            {
                // 장비 해제: 장비 끄고 저장된 Top 복원
                if (_currentArmor != null) _currentArmor.SetActive(false);
                if (_savedTop != null) { _savedTop.SetActive(true); _savedTop = null; }
            }
        }

        // --- Leggings(하의) ---
        if (_leggingsPrefabs != null && classIdx < _leggingsPrefabs.Count)
        {
            if (EquipedLeggings)
            {
                if (_savedBottom == null) _savedBottom = GetActiveChild("Bottom");
                if (_savedBottom != null) _savedBottom.SetActive(false);

                _currentLeggings = _leggingsPrefabs[classIdx];
                _currentLeggings.SetActive(true);
            }
            else
            {
                if (_currentLeggings != null) _currentLeggings.SetActive(false);
                if (_savedBottom != null) { _savedBottom.SetActive(true); _savedBottom = null; }
            }
        }

        // --- Boots ---
        if (_bootsPrefabs != null && classIdx < _bootsPrefabs.Count)
            _bootsPrefabs[classIdx].SetActive(EquipedBoots);
    }
}
