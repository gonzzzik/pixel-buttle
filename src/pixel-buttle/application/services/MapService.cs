using pixel_buttle.domain.interfaces;
using pixel_buttle.domain.models;
using System.Timers;

public class MapService : IMapService, IDisposable
{
    private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
    private readonly IRepository _repository;
    private readonly ITournamentService _tournamentService;
    private readonly Timer _timer;
    private MapModel _model;

    public MapService(ITournamentService tournament, IRepository repository)
    {
        _tournamentService = tournament ?? throw new ArgumentNullException(nameof(tournament));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _model = new MapModel(1000);

        _timer = new Timer(300000);
        _timer.Elapsed += AutoSave;
        _timer.AutoReset = true;
        _timer.Start();
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _rwLock?.Dispose();
    }

    public event Action<(int I, int J, Cell OldCell, Cell NewCell)>? CellChanged;

    private void AutoSave(object source, ElapsedEventArgs e)
    {
        _repository.SaveMap(_model);
        _tournamentService.AutoSave();
    }

    public Cell GetCell(int i, int j)
    {
        _rwLock.EnterReadLock();
        try
        {
            return _model[i, j];
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public void SetCell(Cell cell, int i, int j)
    {
        
        if (!cell.UserID.HasValue) return;

        if (!_tournamentService.CanPerformAction(cell.UserID.Value))
            return;

        _rwLock.EnterWriteLock();
        try
        {
            if (!_tournamentService.CanPerformAction(cell.UserID.Value))
                return;

            _tournamentService.AddPoint(cell.UserID.Value);
            var oldCell = _model[i, j];
            _model[i, j] = cell;
            CellChanged?.Invoke((i, j, oldCell, cell));
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public bool IsValid(int i, int j) => i >= 0 && i < _model.Size && j >= 0 && j < _model.Size;

    public void LoadNewMap(Cell[,] cells, int size)
    {
        var newModel = new MapModel(cells, size);
        _rwLock.EnterWriteLock();
        try
        {
            _model = newModel;
            _tournamentService.ResetTable();
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public void ResetMap(int size)
    {
        var newModel = new MapModel(size);
        _rwLock.EnterWriteLock();
        try
        {
            _model = newModel;
            _tournamentService.ResetTable();
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }
}