/// 업적 메타(불변) + 규칙 (Aggregate Root)
public class Achievement
{
    public int Id { get; }
    public string Title { get; }
    public string Description { get; }
    public string Category { get; }
    public bool Hidden { get; }
    public ICriteriaSpec Criteria { get; }

    public Achievement(int id,
        string title,
        string description,
        string category,
        bool hidden,
        ICriteriaSpec criteria)
    {
        Id = id;
        Title = title;
        Description = description;
        Category = category;
        Hidden = hidden;
        Criteria = criteria;
    }
}