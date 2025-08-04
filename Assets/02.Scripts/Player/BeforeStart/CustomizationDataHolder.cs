using UnityEngine;

public class CustomizationDataHolder : MonoBehaviour
{
    public static CustomizationDataHolder Instance { get; private set; }

    public CustomizationData CustomizationData;
    public string Nickname;
    public ECharacterType ClassType;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}