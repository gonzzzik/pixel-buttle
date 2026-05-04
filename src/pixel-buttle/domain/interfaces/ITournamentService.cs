namespace pixel_buttle.domain.interfaces;

public interface ITournamentService
{
    bool CanPerformAction(Guid userId);
    bool AddPoint(Guid userId);
    bool RemovePoint(Guid userId);
    void ResetTable();
    void ClearById(Guid userId);
    int GetPointCount(Guid userId);
    void AutoSave();
}