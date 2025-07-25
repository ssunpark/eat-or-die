using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcDataManager : BehaviourSingleton<NpcDataManager>
{
    private const string NPC_CSV_PATH = "/NpcCSV/Npc.csv";
    private const string NPCITEM_CSV_PATH = "/NpcCSV/NpcItem.csv";
    private const string NPCDIALOGUE_CSV_PATH = "/NpcCSV/NpcDialogue.csv";
    
    public List<Npc> NpcList { get; private set; }
    public List<NpcItem> NpcItemList { get; private set; }
    public List<NpcDialogue> NpcDialogueList { get; private set; }
    public event Action OnDataLoaded;

    private void Start()
    {
        InitNpcData();
        InitNpcItemData();
        InitNpcDialogueData();
    }

    private void InitNpcData()
    {
        NpcList = CSVLoader<Npc>.LoadCSV(Application.streamingAssetsPath + NPC_CSV_PATH);

        Debug.Log($"로드 완료 - NpcList: {NpcList.Count}, ");
        OnDataLoaded?.Invoke();
    }
    
    private void InitNpcItemData()
    {
        NpcItemList = CSVLoader<NpcItem>.LoadCSV(Application.streamingAssetsPath + NPCITEM_CSV_PATH);

        Debug.Log($"로드 완료 - NpcItemList: {NpcItemList.Count}, ");
        OnDataLoaded?.Invoke();
    }
    
    private void InitNpcDialogueData()
    {
        NpcDialogueList = CSVLoader<NpcDialogue>.LoadCSV(Application.streamingAssetsPath + NPCDIALOGUE_CSV_PATH);

        Debug.Log($"로드 완료 - NpcDialogueList: {NpcDialogueList.Count}, ");
        OnDataLoaded?.Invoke();
    }
}