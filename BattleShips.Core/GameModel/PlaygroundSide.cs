using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

public interface IPlaygroundSide
{
    public ActionResult Bomb(int row, int column);
    public bool HasAliveShips();
    public IPlaygroundShip? PlaceShip(IShip ship, int row, int column, Orientation orientation);
}

public class PlaygroundSide : IPlaygroundSide
{
    private readonly int _rows;
    private readonly int _columns;
    private readonly IPlaygroundShipFactory _playgroundShipFactory;
    private readonly List<IPlaygroundShip> _ships;
    private readonly Dictionary<(int Row, int Column), IPlaygroundShip> _positionToShip;
    private readonly HashSet<(int Row, int Column)> _occupiedPositions;

    public PlaygroundSide(int rows, int columns, IPlaygroundShipFactory playgroundShipFactory)
    {
        if (rows <= 0 || columns <= 0)
            throw new ArgumentException(
                $"PlaygroundSide's rows and columns have to be positive. Provided (r: {rows}, c:{columns})");
        
        _rows = rows;
        _columns = columns;
        _playgroundShipFactory = playgroundShipFactory;
        _ships = new List<IPlaygroundShip>();
        _positionToShip = new Dictionary<(int Row, int Column), IPlaygroundShip>();
        _occupiedPositions = new HashSet<(int Row, int Column)>();
    }

    public ActionResult Bomb(int row, int column)
    {
        var bombOnTarget = _positionToShip.TryGetValue((row, column), out var ship);
        return bombOnTarget ? ship!.Bomb(row, column) : ActionResult.Miss;
    }

    public bool HasAliveShips()
    {
        return _ships.Any(ship => ship.LivePositions.Any());
    }

    public IPlaygroundShip? PlaceShip(IShip ship, int row, int column, Orientation orientation)
    {
        var playgroundShip = _playgroundShipFactory.Create(ship, row, column, orientation);
        if (!playgroundShip.CheckIfShipFits(_rows, _columns) || !playgroundShip.LivePositions.Any())
            return null;

        var shipOverlapsWithOthers = CheckIfShipOverlaps(playgroundShip);
        if (shipOverlapsWithOthers)
            return null;

        AcceptShip(playgroundShip);
        return playgroundShip;
    }

    private bool CheckIfShipOverlaps(IPlaygroundShip playgroundShip) =>
        playgroundShip.LivePositions.Any(position => _occupiedPositions.Contains(position));

    private void AcceptShip(IPlaygroundShip playgroundShip)
    {
        _ships.Add(playgroundShip);
        foreach (var placementPosition in playgroundShip.LivePositions)
        {
            _occupiedPositions.Add(placementPosition);
            _positionToShip[placementPosition] = playgroundShip;
        }
    }
}