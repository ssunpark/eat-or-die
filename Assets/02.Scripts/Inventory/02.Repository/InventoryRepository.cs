using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class InventoryRepository
{
    private readonly FirebaseFirestore _db;
    public FirebaseFirestore DB => _db;

    public InventoryRepository(FirebaseFirestore db)
    {
        _db = db;
    }

    public async UniTask<List<SlotDTO>> LoadInventoryItemList(string userId, string characterId)
    {
        Debug.Log("Try Load InventoryItemList");
        List<SlotDTO> result = new List<SlotDTO>();

        try
        {
            CollectionReference collection = _db.Collection("Users")
                .Document(userId)
                .Collection("Characters")
                .Document(characterId)
                .Collection("Inventory");
            
            QuerySnapshot snapshot = await collection.GetSnapshotAsync();
            foreach (DocumentSnapshot item in snapshot.Documents)
            {
                result.Add(item.ConvertTo<SlotDTO>());
            }
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}");
        }
        
        return result;
    }

    public async UniTask SaveInventoryItem(string userId, string characterId, SlotDTO slotDTO)
    {
        try
        {
            DocumentReference document = _db.Collection("Users")
                .Document(userId)
                .Collection("Characters")
                .Document(characterId)
                .Collection("Inventory")
                .Document(slotDTO.SlotId);

            await document.SetAsync(slotDTO);
        }
        catch (Exception e)
        {
            Debug.Log($"{e.Message}");
        }
    }
}