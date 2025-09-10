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

    public void SelectCharacter(int index)
    {
        _characterInfo = new CharacterInfo(_characterInfoDTOList[index]);
    }
}