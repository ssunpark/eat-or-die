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
        // 1) 클래스 변경 시, 기존 장비/저장본 리셋
        ResetEquipVisualCache();
        DeactivateAllArmorPrefabs();

        // 2) 클래스 설치 및 커스터마이즈 적용
        TryInstall();
        ApplyCustomization();   // 내부에서 SetArmor() 호출됨
    }


    private void ResetEquipVisualCache()
    {
        if (_currentArmor != null) { _currentArmor.SetActive(false); _currentArmor = null; }
        if (_currentLeggings != null) { _currentLeggings.SetActive(false); _currentLeggings = null; }
        _savedTop = null;
        _savedBottom = null;
    }

    private void DeactivateAllArmorPrefabs()
    {
        if (_helmetPrefabs != null) foreach (var go in _helmetPrefabs) if (go) go.SetActive(false);
        if (_armorPrefabs != null) foreach (var go in _armorPrefabs) if (go) go.SetActive(false);
        if (_leggingsPrefabs != null) foreach (var go in _leggingsPrefabs) if (go) go.SetActive(false);
        if (_bootsPrefabs != null) foreach (var go in _bootsPrefabs) if (go) go.SetActive(false);
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

        TryInstall();          // 클래스 모델 설치
        ApplyCustomization();  // Top/Bottom 등 커스터마이즈 확정 (내부에서 SetArmor 호출)

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

        // 인덱스 가드
        bool HasIndex(IList<GameObject> list) => list != null && classIdx >= 0 && classIdx < list.Count;

        // 1) 모든 클래스 아머를 비활성화(겹침 방지)
        if (_helmetPrefabs != null) for (int i = 0; i < _helmetPrefabs.Count; i++) if (_helmetPrefabs[i]) _helmetPrefabs[i].SetActive(false);
        if (_armorPrefabs != null) for (int i = 0; i < _armorPrefabs.Count; i++) if (_armorPrefabs[i]) _armorPrefabs[i].SetActive(false);
        if (_leggingsPrefabs != null) for (int i = 0; i < _leggingsPrefabs.Count; i++) if (_leggingsPrefabs[i]) _leggingsPrefabs[i].SetActive(false);
        if (_bootsPrefabs != null) for (int i = 0; i < _bootsPrefabs.Count; i++) if (_bootsPrefabs[i]) _bootsPrefabs[i].SetActive(false);

        // 2) Top/Bottom 현재 활성을 먼저 확정/저장 (아머를 켜기 전에!)
        if (EquipedArmor)
        {
            if (_savedTop == null)
            {
                var top = GetActiveChild("Top");
                if (top != null) { _savedTop = top; _savedTop.SetActive(false); }
            }
        }
        else
        {
            if (_currentArmor != null) _currentArmor.SetActive(false);
            if (_savedTop != null) { _savedTop.SetActive(true); _savedTop = null; }
        }

        if (EquipedLeggings)
        {
            if (_savedBottom == null)
            {
                var bottom = GetActiveChild("Bottom");
                if (bottom != null) { _savedBottom = bottom; _savedBottom.SetActive(false); }
            }
        }
        else
        {
            if (_currentLeggings != null) _currentLeggings.SetActive(false);
            if (_savedBottom != null) { _savedBottom.SetActive(true); _savedBottom = null; }
        }

        // 3) 이제 아머/부츠를 켠다 (현재 클래스 슬롯만)
        if (HasIndex(_helmetPrefabs)) _helmetPrefabs[classIdx].SetActive(EquipedHelmet);

        if (HasIndex(_armorPrefabs))
        {
            _currentArmor = _armorPrefabs[classIdx];
            _currentArmor.SetActive(EquipedArmor);
        }

        if (HasIndex(_leggingsPrefabs))
        {
            _currentLeggings = _leggingsPrefabs[classIdx];
            _currentLeggings.SetActive(EquipedLeggings);
        }

        if (HasIndex(_bootsPrefabs)) _bootsPrefabs[classIdx].SetActive(EquipedBoots);
    }
}
