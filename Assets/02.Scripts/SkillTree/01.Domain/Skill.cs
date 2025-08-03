using System;

public class Skill
{
    public readonly int Index;
    
    // 스킬 도메인
    public Skill(int index)
    {
        if (index < 0 || index > 5)
        {
            throw new Exception($"{index}는 유효하지 않은 스킬의 인덱스입니다. 스킬의 인덱스는 1~5입니다.");
        }
        
        Index = index;
    }
}
