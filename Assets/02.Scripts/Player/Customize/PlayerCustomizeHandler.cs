using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
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
    public ECharacterType ClassType => _classType;
    [Networked, OnChangedRender(nameof(ApplyNickname))] public string Nickname { get; set; }
    [Networked, OnChangedRender(nameof(ApplyCustomization))] private CustomizationData _customData { get; set; }

    [SerializeField] private TextMeshProUGUI _nicknameText;

    private Dictionary<ECustomizationPart, int> _customizeSelections = new();
    public override void Spawned()
    {

        if (Object.HasInputAuthority)
        {
            if(CustomizationDataHolder.Instance == null)
            {
                Debug.LogWarning("[PlayerCustomizeHandler] CustomizationDataHolder가 없어용");
                Debug.LogWarning("캐릭터 커스터마이징이 안되니 참고하세용");
                //커스터마이징 데이터가 없으면 그냥 기본값으로 설정
                var classType = ECharacterType.Warrior; // 기본 클래스
                var nickname = "Player"+UnityEngine.Random.Range(100,999); // 기본 닉네임
                var customData = new CustomizationData(); // 빈 커스터마이징 데이터
                Rpc_SetCharacterInfo(classType, nickname, customData);
                return;
            }
            var holder = CustomizationDataHolder.Instance;

            if (string.IsNullOrEmpty(holder.Nickname))
            {
                holder.Nickname = "Player"+ UnityEngine.Random.Range(100, 999);
            }
            Rpc_SetCharacterInfo(holder.ClassType, holder.Nickname, holder.CustomizationData);
            SendNicknameToHost();
        }
        else
        {
            //후입장 플레이어를 위해 한 번 강제 적용
            ApplyCustomization();
            ApplyNickname();
        }
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

    private void ApplyNickname()
    {
        if(string.IsNullOrEmpty(Nickname))
        {
            Nickname = "Player";
        }
        if (_nicknameText != null)
        {
            _nicknameText.text = Nickname;
        }
    }
    public void ApplyBtn()
    {
        if (Object.HasInputAuthority)
        {
            CustomizationData data = CustomizationDataMapper.FromDictionary(_customizeSelections);
            Rpc_SetCharacterInfo(_classType, Nickname, data);
        }
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
        Nickname = nickname;
        _customData = data;
        ApplyCustomization();
    }

    public void SendNicknameToHost()
    {
        if (Object.HasStateAuthority)
        {
            var holder = CustomizationDataHolder.Instance;
            string nickname = string.IsNullOrEmpty(holder.Nickname) ? "Player" + UnityEngine.Random.Range(100, 999) : holder.Nickname;
            CustomizationData data = holder.CustomizationData;
            Nickname = nickname;
            _customData = data;

            PlayerInfoManager.Instance.UpdateNickname(Object.InputAuthority, nickname);
            return;
        }
        if (Object.HasInputAuthority)
        {
            var holder = CustomizationDataHolder.Instance;
            string nickname = string.IsNullOrEmpty(holder.Nickname) ? "Player" + UnityEngine.Random.Range(100, 999) : holder.Nickname;
            CustomizationData data = holder.CustomizationData;

            Rpc_SendNicknameAndCustomization(nickname, data);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void Rpc_SendNicknameAndCustomization(string nickname, CustomizationData data)
    {
        Nickname = nickname;
        _customData = data;

        PlayerInfoManager.Instance.UpdateNickname(Object.InputAuthority, nickname);
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
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
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
