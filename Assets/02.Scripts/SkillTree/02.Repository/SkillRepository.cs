using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillRepository
{
    private const string SKILL_CSV_PATH = "/SkillCSV/Skill.csv";
    private const string SAVE_FILE_NAME = "/SkillData/skills.json";

    [Serializable]
    private class SkillSave
    {
        public List<SkillDTO> skills = new();
    }

    public List<Skill> LoadSkillRawDataList()
    {
        List<Skill> result = new List<Skill>();
        var rawdatas = CSVLoader<SkillRawData>.LoadCSV($"{Application.streamingAssetsPath}{SKILL_CSV_PATH}");
        foreach (var meta in rawdatas)
            result.Add(new Skill(meta));
        return result;
    }
    
    public void SaveSkillDataList(IEnumerable<Skill> allSkills)
    {
        var save = new SkillSave();

        foreach (var s in allSkills)
        {
            if (s.Level > 0) // 0레벨은 저장 안 함
                save.skills.Add(new SkillDTO(s.Meta.Id, s.Level));
        }

        var json = JsonUtility.ToJson(save);
        var path = $"{Application.streamingAssetsPath}{SAVE_FILE_NAME}";
        File.WriteAllText(path, json);
#if UNITY_EDITOR
        Debug.Log($"[SkillRepository] Saved {save.skills.Count} skills -> {path}");
#endif
    }

    public List<SkillDTO> LoadSkillDataList()
    {
        var path = $"{Application.streamingAssetsPath}{SAVE_FILE_NAME}";
        Debug.Log($"[SkillRepository] Loading {path}");
        if (!File.Exists(path))
            return new List<SkillDTO>();

        try
        {
            var json = File.ReadAllText(path);
            var save = JsonUtility.FromJson<SkillSave>(json);
            return save?.skills ?? new List<SkillDTO>();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SkillRepository] Load failed: {e.Message}");
            return new List<SkillDTO>();
        }
    }
}