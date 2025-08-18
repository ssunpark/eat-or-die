using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int StageIndex;
    
    public void Enter()
    {
        Debug.Log($"Stage {StageIndex} Enter");
    }

    public void Exit()
    {
        Debug.Log($"Stage {StageIndex} Exit");
    }
}
