using System;
using Firebase.Firestore;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class CharacterInfoRepository
{
    private readonly FirebaseFirestore _db;
    public FirebaseFirestore DB => _db;

    public CharacterInfoRepository(FirebaseFirestore db)
    {
        _db = db;
    }

    public async UniTask CreateNewCharacterDocument(CharacterInfoDTO characterInfoDTO, string userId)
    {
        try
        {
            DocumentReference docRef = _db.Collection("Users")
                .Document(userId)
                .Collection("Characters")
                .Document();

            characterInfoDTO.Id = docRef.Id;
            await docRef.SetAsync(characterInfoDTO);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public async UniTask<List<CharacterInfoDTO>> LoadCharacterInfoListAsync(string userId)
    {
        List<CharacterInfoDTO> characterInfoList = new List<CharacterInfoDTO>();

        try
        {
            Query query = _db.Collection("Users")
                .Document(userId)
                .Collection("Characters")
                .OrderByDescending("LastLoginAt");

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot document in querySnapshot.Documents)
            {
                characterInfoList.Add(document.ConvertTo<CharacterInfoDTO>());
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Fail to load character info: {e.Message}");
        }
        
        return characterInfoList;
    }
    
    public async UniTask<CharacterInfoDTO> LoadCharacterInfoAsync(string userId, string characterId)
    {
        DocumentReference docRef = _db.Collection("Users")
            .Document(userId)
            .Collection("Characters")
            .Document(characterId);
        
        try
        {
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                CharacterInfoDTO data = snapshot.ConvertTo<CharacterInfoDTO>();
                return data;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fail to load character info: {ex.Message}");
            return null;
        }
    }
}