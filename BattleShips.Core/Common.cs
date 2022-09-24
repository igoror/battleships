namespace BattleShips.Core;

public enum PlayerType
{
    First,
    Second
}

public enum Orientation
{
    Horizontal, Vertical
}

public enum CellStatus
{
    Empty,
    ShipPart,
    DestroyedShipPart,
    Missed,
}

public static class Extensions
{
    public static PlayerType Other(this PlayerType playerType) =>
        playerType == PlayerType.First ? PlayerType.Second : PlayerType.First;

    public static bool InRange(this int val, int min, int max) => min <= val && val <= max;

    public static bool IsUpperLetter(this char c) =>((int) c).InRange('A', 'Z');
    public static bool IsLowerLetter(this char c) =>((int) c).InRange('a', 'z');
    public static bool IsLetter(this char c) => c.IsUpperLetter() || c.IsLowerLetter();
}

public static class Constants
{
    public const int MaxDimensionSizeWithHumanAsPlayer = 26;
    public static readonly (int Row, int Column, Orientation rotation) FailureShipPlacement =
        (-1, -1, Orientation.Horizontal);
    public static readonly (int Row, int Column) FailureBombingPosition = (-1, -1);
}

public static class Utils
{
    public static int ParseRow(char row)
    {
        if (row.IsLowerLetter())
            return row - 'a';
        return row - 'A';
    }
}