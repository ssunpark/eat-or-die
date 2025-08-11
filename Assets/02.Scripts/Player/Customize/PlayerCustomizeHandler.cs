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
}
