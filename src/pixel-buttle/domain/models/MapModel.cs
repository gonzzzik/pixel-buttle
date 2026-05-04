using System.Runtime.CompilerServices;

namespace pixel_buttle.domain.models;

public class MapModel
{
    private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
    private Cell[,] _cells;
    public Guid UUID { get; init; } = Guid.NewGuid();
    public int Size { get; init; }


    public MapModel(Cell[,] cells, int size)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException($"Error. Size < 0: {size}");
        else if (cells.GetLength(0) != size)
            throw new ArgumentException($"Error. Size != Map.Size: {size} != {cells.GetLength(0)}");
        else if (cells.GetLength(1) != cells.GetLength(0))
            throw new ArgumentException($"Error. Cells Array is not square: {cells.GetLength(0)} != {cells.GetLength(1)}");

        _cells = cells;
        Size = size;
    }

    public MapModel(int size) : this(new Cell[size, size], size) { }

    public void Dispose()
    {
        _rwLock?.Dispose();
    }


    public Cell this[int i, int j]
    {
        get
        {
            if (i < 0 || i >= Size || j < 0 || j >= Size) 
                throw new ArgumentOutOfRangeException($"Error. Get in {UUID}. Index i ({i}) or/and j ({j}) not in range 0..{Size - 1}");

            _rwLock.EnterReadLock();
            try { return _cells[i, j]; }
            finally { _rwLock.ExitReadLock(); }
        }
        set
        {
            if (i < 0 || i >= Size || j < 0 || j >= Size) 
                throw new ArgumentOutOfRangeException($"Error. Set in {UUID}. Index i ({i}) or/and j ({j}) not in range 0..{Size - 1}");

            _rwLock.EnterWriteLock();
            try { _cells[i, j] = value; }
            finally { _rwLock.ExitWriteLock(); }
        }
    }

}