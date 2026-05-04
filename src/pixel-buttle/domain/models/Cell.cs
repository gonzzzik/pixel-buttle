namespace pixel_buttle.domain.models
{
    public readonly record struct Cell
    {
        public int Red { get; init; }
        public int Green { get; init; }
        public int Blue { get; init; }
        public Guid? UserID { get; init; }
        public bool IsLocked { get; init; }

        public Cell(int red, int green, int blue, Guid? userId = null, bool isLocked = false)
        {
            Red = CheckRange(red);
            Green = CheckRange(green);
            Blue = CheckRange(blue);
            UserID = userId;
            IsLocked = isLocked;
        }

        private static int CheckRange(int value) =>
            value is >= 0 and <= 255 ? value : throw new ArgumentOutOfRangeException($"Error. nameof(value) must in range 0..255.");
    }
}
