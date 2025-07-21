using UnityEngine;

public class CharacterStatInstaller : MonoBehaviour
{
    [SerializeField] private bool useMockData = true;
    [SerializeField] private bool syncStats = false;
    [SerializeField] private bool syncResources = false;

    private void Awake()
    {
        IStatDataRepository repo = useMockData
            ? new MockStatDataRepository()
            : new StatDataRepository();

        var character = GetComponent<CharacterBase>();
        character.InitializeStat(repo);

        if (syncStats)
        {
            var statSync = GetComponent<CharacterStatNetworkSync>();
            if (statSync != null)
                statSync.Initialize(character.Stat);
        }

        if (syncResources)
        {
            var resourceSync = GetComponent<CharacterResourceNetworkSync>();
            if (resourceSync != null)
                resourceSync.Initialize(character.Resource);
        }

        var debugger = GetComponent<PlayerStatDebugger>();
        if (debugger != null)
            debugger.Bind(character.Stat);
    }
}
