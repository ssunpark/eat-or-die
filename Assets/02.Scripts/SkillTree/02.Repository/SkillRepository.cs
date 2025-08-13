using System.Collections.Generic;
using UnityEngine;

public class SkillRepository
{
    private const string SKILL_CSV_PATH = "/SkillCSV/Skill.csv";

    public List<Skill> LoadSkillRawDataList()
    {
        List<Skill> result = new List<Skill>();
        var rawdatas = CSVLoader<SkillRawData>.LoadCSV($"{Application.streamingAssetsPath}{SKILL_CSV_PATH}");
        foreach (var meta in rawdatas)
        {
            result.Add(new Skill(meta));
        }
        
        return result;
    }
}