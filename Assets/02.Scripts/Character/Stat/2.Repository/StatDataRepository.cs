using System.Collections.Generic;
using UnityEngine;
public class StatDataRepository : IStatDataRepository
{
    public List<CharacterStatData> GetCharacterStatData(ECharacterType type)
    {
        Dictionary<EStatType, float> baseStats = CharacterStatPreset.GetBaseStats(type);
        List<CharacterStatData> statDataList = new List<CharacterStatData>();
        foreach (var stat in baseStats)
        {
            statDataList.Add(new CharacterStatData(stat.Key, stat.Value));
        }
        return statDataList;
    }
}
