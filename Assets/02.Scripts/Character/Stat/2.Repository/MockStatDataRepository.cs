using System.Collections.Generic;

public class MockStatDataRepository : IStatDataRepository
{
    public List<CharacterStatData> GetCharacterStatData(ECharacterType type) => MockStatDataTable.GetMockData();
}