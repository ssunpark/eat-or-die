using UnityEngine;

public class CharacterDataInstaller : MonoBehaviour
{
    [SerializeField] private bool useMockStatData = true;
    [SerializeField] private bool useMockTraitData = true;
    [SerializeField] private bool syncStats = false;
    [SerializeField] private bool syncTraits = false;
    [SerializeField] private bool syncResources = false;

    private void Awake()
    {
        IStatDataRepository statRepo = useMockStatData
            ? new MockStatDataRepository()
            : new StatDataRepository(); // Todo: 실제 Stat 데이터 레포지토리 작성 예정

        ITraitDataRepository traitRepo = useMockTraitData
            ? new MockTraitDataRepository()
            : null; // Todo: 실제 Trait 데이터 레포지토리 작성 예정

        var character = GetComponent<CharacterBase>();
        character.InitializeCharacter(statRepo, traitRepo);

        // Trait 효과 Stat에 반영
        character.Trait.ReapplyAllTraitEffects(traitRepo.GetCharacterTraitData());

        if (syncStats)
        {
            var statSync = GetComponent<CharacterStatNetworkSync>();
            if (statSync != null)
                statSync.Initialize(character.Stat);
        }

        if (syncTraits)
        {
            var traitSync = GetComponent<CharacterTraitNetworkSync>();
            if (traitSync != null)
                traitSync.Initialize(character.Trait);
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
