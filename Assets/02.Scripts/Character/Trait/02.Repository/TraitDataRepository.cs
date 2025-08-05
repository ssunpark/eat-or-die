using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TraitDataRepository : ITraitDataRepository
{
    private const string TRAIT_CSV_PATH = "/PlayerCSV/Trait.csv";
    private List<CharacterTraitData> _cachedData;

    public List<CharacterTraitData> GetCharacterTraitData()
    {
        try {
            if (_cachedData != null)
                return _cachedData;

            string fullPath = Path.Combine(Application.streamingAssetsPath, TRAIT_CSV_PATH);
            var rawList = CSVLoader<CharacterTraitRawData>.LoadCSV(fullPath);

            _cachedData = rawList
                .Select(raw => new CharacterTraitData(
                    raw.ID,
                    raw.TraitType,
                    raw.Name,
                    raw.Description,
                    raw.ModifierType,
                    raw.MaxLevel,
                    raw.StatType,
                    raw.ValuePerLevel,
                    raw.ActionName,
                    raw.ExpValue,
                    raw.ExpPerLevel,
                    raw.IconPath
                ))
                .ToList();

            return _cachedData;
        }
        catch(FileNotFoundException e) {
            Debug.LogError($"[TraitDataRepository] CSV 파일을 찾을 수 없습니다: {e.Message}");
            return MockTraitDataTable.GetMockData();
        }
        catch (IOException e) {
            Debug.LogError($"[TraitDataRepository] CSV 파일 로드 중 오류 발생: {e.Message}");
            return MockTraitDataTable.GetMockData();
        }
        catch (System.Exception e)  // 예기치 않은 오류 처리
        {
            Debug.LogError($"[TraitDataRepository] CSV 로드 중 예기치 않은 오류 발생: {e.Message}");
            Debug.LogWarning("[TraitDataRepository] CSV 로드 실패 - Mock 데이터 반환");
            return MockTraitDataTable.GetMockData();
        }
        
    }
}
