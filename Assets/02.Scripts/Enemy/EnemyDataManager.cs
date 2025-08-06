using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyDataManager : BehaviourSingleton<EnemyDataManager>
{
    public const string ENEMY_CSV_PATH = "/EnemyCSV/Enemy.csv";
    
    private List<EnemyRawData> _enemyRawDataList;
    public Dictionary<int, EnemyRawData> EnemyRawDataDictionary;

    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        _enemyRawDataList = CSVLoader<EnemyRawData>.LoadCSV($"{Application.streamingAssetsPath}{ENEMY_CSV_PATH}");
        EnemyRawDataDictionary = _enemyRawDataList.ToDictionary(item => item.ID);
    }
}