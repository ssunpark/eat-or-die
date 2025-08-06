using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    public DragonStateParameterSet.ChaseParams Chase => _parameters.Chase;
    public DragonStateParameterSet.AttackParams Attack => _parameters.Attack;
    public DragonStateParameterSet.PrepareParams Prepare => _parameters.Prepare;
    public DragonStateParameterSet.SwipeParams Swipe => _parameters.Swipe;
    public DragonStateParameterSet.RightScratchParams RightScratch => _parameters.RightScratch;
    public DragonStateParameterSet.LeftScratchParams LeftScratch => _parameters.LeftScratch;
    public DragonStateParameterSet.BiteParams Bite => _parameters.Bite;
    public DragonStateParameterSet.MagicParams Magic => _parameters.Magic;
    public DragonStateParameterSet.BreathParams Breath => _parameters.Breath;
    public DragonStateParameterSet.LavaParams Lava => _parameters.Lava;
    public DragonStateParameterSet.RoarParams Roar => _parameters.Roar;
}