using System.Collections.Generic;

public class MockStatDataRepository : IStatDataRepository
{
    public List<PlayerStatData> GetPlayerStatData() => MockStatDataTable.GetMockData();
}