using System;
using Firebase.Firestore;
using UnityEngine;

[Serializable]
public class CharacterInfo
{
    [Header("캐릭터 정보")]
    public string Id;
    public string Name;
    public Timestamp CreatedAt;
    public Timestamp LastLoginAt;

    public CharacterInfo(CharacterInfoDTO characterInfoDTO)
    {
        Id = characterInfoDTO.Id;
        Name = characterInfoDTO.Name;
        CreatedAt = characterInfoDTO.CreatedAt;
        LastLoginAt = characterInfoDTO.LastLoginAt;
    }
}