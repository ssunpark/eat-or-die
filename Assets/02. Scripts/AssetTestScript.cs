using UnityEngine;

public class AssetTestScript : MonoBehaviour
{
    public GameObject TestPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(TestPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
