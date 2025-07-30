using System.IO;
using UnityEngine;

public class DragonParameterLoader
{
    private const string BOSS_JSON_PATH = "BossJson/DragonStateParameters.json";
    private DragonStateParameterSet _parameters;

    public DragonParameterLoader()
    {
        LoadParameters();
    }

    private void LoadParameters()
    {
        string path = Path.Combine(Application.streamingAssetsPath, BOSS_JSON_PATH);

        string json = File.ReadAllText(path);

        _parameters = JsonUtility.FromJson<DragonStateParameterSet>(json);
    }

    public DragonStateParameterSet.BaseParams Base => _parameters.Base;
    public DragonStateParameterSet.PatrolParams Patrol => _parameters.Patrol;
    public DragonStateParameterSet.WaitParams Wait => _parameters.Wait;
    public DragonStateParameterSet.AlertParams Alert => _parameters.Alert;
    public DragonStateParameterSet.SwipeParams Swipe => _parameters.Swipe;
}