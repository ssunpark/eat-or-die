using UnityEngine;
/// <summary>
/// 캐릭터 데이터 초기화 및 설정을 담당하는 컴포넌트입니다.
/// Stat: 직업별 기본 스탯 데이터를 프리셋에서 가져옵니다.
/// Trait: 플레이어가 찍어놓은 Trait 데이터를 가져옵니다.
/// </summary>
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
            : new StatDataRepository();

        ITraitDataRepository traitRepo = useMockTraitData
            ? new MockTraitDataRepository()
            : new TraitDataRepository();

        var character = GetComponent<CharacterBase>();
        character.InitializeCharacter(statRepo, traitRepo);

        // Trait 효과 Stat에 반영
        character.Trait.ReapplyAllTraitEffects(traitRepo.GetCharacterTraitData());

        if (character is Player player)
        {
            var traitData = traitRepo.GetCharacterTraitData();
            player.InitializeTraitSystem(traitData, new TraitExpHandler(traitData, player.Trait));
        }

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

        TryGetComponent(out PlayerStatDebugger debugger);
        if (debugger != null)
            debugger.Bind(character.Stat);
    }
}
