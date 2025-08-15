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
    private bool _installed;

    public void Install(ECharacterType characterType)
    {
        if (_installed) return;

        IStatDataRepository statRepo = useMockStatData ? new MockStatDataRepository() : new StatDataRepository();
        ITraitDataRepository traitRepo = useMockTraitData ? new MockTraitDataRepository() : new TraitDataRepository();

        var character = GetComponent<CharacterBase>();

        character.InitializeCharacter(statRepo, traitRepo, characterType);
        if (character is Player player)
        {
            var traitData = traitRepo.GetCharacterTraitData();
            player.InitializeTraitSystem(traitData, new TraitExpHandler(traitData, player.Trait));
        }
        character.Trait.ReapplyAllTraitEffects(traitRepo.GetCharacterTraitData());

        if (syncStats)
            GetComponent<CharacterStatNetworkSync>()?.Initialize(character.Stat);
        if (syncTraits)
            GetComponent<CharacterTraitNetworkSync>()?.Initialize(character.Trait);
        if (syncResources)
            GetComponent<CharacterResourceNetworkSync>()?.Initialize(character.Resource);

        _installed = true;
    }
}
