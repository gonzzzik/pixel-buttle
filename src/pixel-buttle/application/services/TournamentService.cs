// application/services/TournamentService.cs
namespace pixel_buttle.application.services;

using domain.interfaces;
using domain.models;

public class TournamentService : ITournamentService
{
    private readonly TournamentData _data = new();
    private readonly TimeSpan _actionCooldown; // например, TimeSpan.FromSeconds(5)
    private readonly IRepository? _repository; // опционально

    public TournamentService(TimeSpan? cooldown = null, IRepository? repository = null)
    {
        _actionCooldown = cooldown ?? TimeSpan.Zero;
        _repository = repository;
    }

    public bool CanPerformAction(Guid userId) => _data.GetPoints(userId) != default;

    public bool AddPoint(Guid userId)
    {
        var result = _data.TryAddPoint(userId, _actionCooldown);
        if (result) _repository?.SaveTournament(_data); // опционально
        return result;
    }

    public bool RemovePoint(Guid userId) => _data.TryRemovePoint(userId);

    public void ResetTable() => _data.Reset();

    public void ClearById(Guid userId) => _data.ClearUser(userId);

    public int GetPointCount(Guid userId) => _data.GetPoints(userId);

    public void AutoSave() => _repository?.SaveTournament(_data);
}