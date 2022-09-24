namespace BattleShips.Core.Ships;


// Defines straight ship: X, XX, XXX, ...
public abstract class SimpleShip : IShip
{
    protected abstract int Length { get; }

    public IEnumerable<(int, int)> PlacementPositions()
    {
        for (var i = 0; i < Length; ++i)
            yield return (0, i);
    }

    public (int Width, int Height) OccupiedArea => (Length, 1);
}