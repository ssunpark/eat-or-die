using System.Collections.Generic;

public class MockTraitDataRepository : ITraitDataRepository
{
    public List<CharacterTraitData> GetCharacterTraitData() => MockTraitDataTable.GetMockData();
}
