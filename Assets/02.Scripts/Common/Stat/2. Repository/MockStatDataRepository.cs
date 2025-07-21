using System.Collections.Generic;

public class MockStatDataRepository : IStatDataRepository
{
    public List<CharacterStatData> GetCharacterStatData() => MockStatDataTable.GetMockData();
}