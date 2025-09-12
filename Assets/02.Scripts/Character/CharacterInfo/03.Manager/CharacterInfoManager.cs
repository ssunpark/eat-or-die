using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;

public class CharacterInfoManager : BehaviourSingleton<CharacterInfoManager>
{
    private CharacterInfoRepository _repository;
    private List<CharacterInfoDTO> _characterInfoDTOList;
    public List<CharacterInfoDTO> CharacterInfoDTOList => _characterInfoDTOList;
    
    private CharacterInfo _characterInfo = null;
    public CharacterInfo CharacterInfo => _characterInfo;
    
    public bool IsInitialized => _characterInfo != null;
    
    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await FirebaseManager.Instance.WaitForInitialization();
        
        _repository = new CharacterInfoRepository(FirebaseManager.Instance.DB);
        AuthenticationManager.Instance.OnLogin += InitializeCharacterInfos;
    }

    private async void InitializeCharacterInfos()
    {
        _characterInfoDTOList = await _repository.LoadCharacterInfoListAsync(AuthenticationManager.Instance.User.UserId);
        Debug.Log(_characterInfoDTOList.Count + " character info loaded");
    }
    
    public async void CreateNewCharacter(CharacterInfoDTO characterInfoDTO)
    {
        await _repository.CreateNewCharacterDocument(characterInfoDTO, AuthenticationManager.Instance.User.UserId);
        
        _characterInfo = new CharacterInfo(characterInfoDTO);
        SetCharacterInfo();
    }

    public void SelectCharacter(int index)
    {
        _characterInfo = new CharacterInfo(_characterInfoDTOList[index]);
        SetCharacterInfo();
    }

    private void SetCharacterInfo()
    {
        CustomizationDataHolder data = CustomizationDataHolder.Instance;
        CustomizationData custom = new CustomizationData();
        
        custom.Top = (short)_characterInfo.Top;
        custom.Bottom = (short)_characterInfo.Bottom;
        custom.Hair = (short)_characterInfo.Hair;
        custom.Eye = (short)_characterInfo.Eye;

        data.Nickname = _characterInfo.Name;
        data.ClassType = _characterInfo.Class;
        data.CustomizationData = custom;

        if (AchievementManager.Instance == null)
        {
            Debug.LogError("No AchievementManager found");
        }
        else
        {
            var achievementRepo = new FirestorePlayerAchievementRepository(FirebaseManager.Instance.DB, AuthenticationManager.Instance.User.UserId, _characterInfo.Id);
            AchievementManager.Instance.SetRepository(achievementRepo);
        }
    }
}