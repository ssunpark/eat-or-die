using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : MonoBehaviour
{
    // Async라 쓰고 즉시 로드 때려버리기~
    void Start()
    {
        SceneManager.LoadScene(2);
    }

     void Update()
    {
        
    }
}
