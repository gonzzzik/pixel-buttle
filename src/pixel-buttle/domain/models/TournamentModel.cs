namespace pixel_buttle.domain.models;

public class TournamentData
{
    private readonly Dictionary<Guid, int> _points = new();
    private readonly Dictionary<Guid, DateTime> _lastActionTime = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public int GetPoints(Guid userId)
    {
        _lock.EnterReadLock();
        try { return _points.GetValueOrDefault(userId); }
        finally { _lock.ExitReadLock(); }
    }

    public bool TryAddPoint(Guid userId, TimeSpan cooldown)
    {
        _lock.EnterUpgradeableReadLock();
        try
        {
            var now = DateTime.UtcNow;
            if (_lastActionTime.TryGetValue(userId, out var lastTime) && now - lastTime < cooldown)
                return false;

            _lock.EnterWriteLock();
            try
            {
                _points[userId] = _points.GetValueOrDefault(userId) + 1;
                _lastActionTime[userId] = now;
                return true;
            }
            finally { _lock.ExitWriteLock(); }
        }
        finally { _lock.ExitUpgradeableReadLock(); }
    }

    public bool TryRemovePoint(Guid userId)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!_points.ContainsKey(userId)) return false;
            _points[userId]--;
            if (_points[userId] <= 0) _points.Remove(userId);
            return true;
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void Reset()
    {
        _lock.EnterWriteLock();
        try
        {
            _points.Clear();
            _lastActionTime.Clear();
        }
        finally { _lock.ExitWriteLock(); }
    }

    public void ClearUser(Guid userId)
    {
        _lock.EnterWriteLock();
        try
        {
            _points.Remove(userId);
            _lastActionTime.Remove(userId);
        }
        finally { _lock.ExitWriteLock(); }
    }

    public IReadOnlyDictionary<Guid, int> GetAllPoints()
    {
        _lock.EnterReadLock();
        try { return new Dictionary<Guid, int>(_points); }
        finally { _lock.ExitReadLock(); }
    }
}