using System.Collections.Generic;

public interface IAchievementCatalog {
    public IReadOnlyList<Achievement> GetAll();
    public bool TryGet(int id, out Achievement ach);
}