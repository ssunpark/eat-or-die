using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class SkillRepository
{
    private const string SKILL_CSV_PATH = "/SkillCSV/Skill.csv";

    private readonly FirebaseFirestore _db;
    private readonly string _userId;
    private readonly string _characterId;

    public SkillRepository(FirebaseFirestore db, string userId, string characterId)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _userId = userId ?? throw new ArgumentNullException(nameof(userId));
        _characterId = characterId ?? throw new ArgumentNullException(nameof(characterId));
    }

    private CollectionReference SkillsCol =>
        _db.Collection("Users").Document(_userId)
           .Collection("Characters").Document(_characterId)
           .Collection("Skills");

    // CSV 메타 로딩(그대로 유지)
    public List<Skill> LoadSkillRawDataList()
    {
        var result = new List<Skill>();
        var metas = CSVLoader<SkillRawData>.LoadCSV($"{Application.streamingAssetsPath}{SKILL_CSV_PATH}");
        foreach (var meta in metas)
            result.Add(new Skill(meta));
        return result;
    }

    // 저장: 레벨 > 0만 저장, 나머지는 삭제(배치)
    public async UniTask SaveSkillDataListAsync(IEnumerable<Skill> allSkills)
    {
        if (allSkills == null) return;

        // 현재 문서 목록
        var currentSnap = await SkillsCol.GetSnapshotAsync();
        var existingIds = new HashSet<string>(currentSnap.Documents.Select(d => d.Id));

        // 저장 대상(레벨>0)
        var toSave = allSkills
            .Where(s => s.Level > 0)
            .Select(s => new SkillDTO(s.Meta.Id, s.Level))
            .ToList();

        var saveIds = new HashSet<string>(toSave.Select(t => t.DocId));

        // 배치 커밋
        var batch = _db.StartBatch();

        // upsert
        foreach (var dto in toSave)
        {
            var doc = SkillsCol.Document(dto.DocId);
            batch.Set(doc, dto, SetOptions.MergeAll);
        }

        // 레벨0이 되어 저장 목록에서 빠진 기존 문서 삭제
        foreach (var staleId in existingIds)
        {
            if (!saveIds.Contains(staleId))
                batch.Delete(SkillsCol.Document(staleId));
        }

        await batch.CommitAsync();

#if UNITY_EDITOR
        Debug.Log($"[SkillRepository] Saved {toSave.Count} skills to Firestore.");
#endif
    }

    // 로드: 컬렉션 전체 → DTO 리스트
    public async UniTask<List<SkillDTO>> LoadSkillDataListAsync()
    {
        try
        {
            var snap = await SkillsCol.GetSnapshotAsync();
            var list = new List<SkillDTO>();
            foreach (var doc in snap.Documents)
            {
                // 문서 ID ↔ DocId, 필드 ↔ Level 자동 매핑
                var dto = doc.ConvertTo<SkillDTO>();
                list.Add(dto);
            }
            return list;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SkillRepository] Load failed: {e.Message}");
            return new List<SkillDTO>();
        }
    }
}
