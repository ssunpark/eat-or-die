using CsvHelper.Configuration.Attributes;

public class AchievementRawData
{
    [Name("Id")]
    public int Id { get; set; }

    [Name("Title")]
    public string Title { get; set; } = string.Empty;

    [Name("Description")]
    public string Description { get; set; } = string.Empty;

    [Name("Category")]
    public string Category { get; set; } = string.Empty;

    [Name("Hidden")]
    public bool Hidden { get; set; }

    [Name("CriteriaType")]
    public ECriteriaType CriteriaType { get; set; }

    // CounterReach: statKey / OneShotEvent: eventKey
    [Name("CriteriaKey")]
    public string CriteriaKey { get; set; } = string.Empty;

    // CounterReach에 사용(OneShotEvent는 0 혹은 비워둬도 됨)
    [Name("CriteriaTarget")]
    public long CriteriaTarget { get; set; }

    // (선택) 추가적으로 구분하려는 태그(적 타입 등)
    [Name("Tag")]
    public string? Tag { get; set; }
}