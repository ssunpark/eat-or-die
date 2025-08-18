using Fusion;
using UnityEngine;
using System.IO;

public class ServerModifierBootstrap : NetworkBehaviour
{
    [SerializeField] private string _foodEffectCsv = "ItemCSV/FoodEffect.csv";
    [SerializeField] private string _foodCsv = "ItemCSV/Food.csv";

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        string root = Application.streamingAssetsPath;
        FoodEffectDB.Instance.LoadEffects(Path.Combine(root, _foodEffectCsv));
        FoodDB.Instance.LoadFoods(Path.Combine(root, _foodCsv));
    }
}
