namespace pixel_buttle.domain.interfaces;

using pixel_buttle.domain.models;
using System.Timers;

public interface IMapService
{
    public void SetCell(Cell cell, int i, int j);
    public Cell GetCell(int i, int j);
    public void LoadNewMap(Cell[,] cells, int size);
    public void ResetMap(int size);
    public bool IsValid(int i, int j);
    public void AutoSave(Object source, ElapsedEventArgs e);

    event Action<(int I, int J, Cell OldCell, Cell NewCell)>? CellChanged;
}
