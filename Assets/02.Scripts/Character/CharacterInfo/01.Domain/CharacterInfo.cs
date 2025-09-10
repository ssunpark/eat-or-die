using System;
using Firebase.Firestore;
using UnityEngine;

[Serializable]
public class CharacterInfo
{
    [Header("캐릭터 정보")]
    public string Id { get; private set; }
    public string Name { get; private set; }
    public ECharacterType Class { get; private set; }
    public Timestamp CreatedAt { get; private set; }
    public Timestamp LastLoginAt { get; private set; }
    public int Top { get; private set; }
    public int Bottom { get; private set; }
    public int Hair { get; private set; }
    public int Eye { get; private set; }

    public CharacterInfo(CharacterInfoDTO characterInfoDTO)
    {
        Id = characterInfoDTO.Id;
        Name = characterInfoDTO.Name;
        Class = (ECharacterType)characterInfoDTO.Class;
        CreatedAt = characterInfoDTO.CreatedAt;
        LastLoginAt = characterInfoDTO.LastLoginAt;
        Top = characterInfoDTO.Top;
        Bottom = characterInfoDTO.Bottom;
        Hair = characterInfoDTO.Hair;
        Eye = characterInfoDTO.Eye;
    }
}