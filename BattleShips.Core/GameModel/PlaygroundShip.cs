using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

public interface IPlaygroundShip
{
    bool CheckIfShipFits(int rowsSize, int columnsSize);
    ActionResult Bomb(int row, int column);
    IEnumerable<(int Row, int Column)> LivePositions { get; }
}

public class PlaygroundShip : IPlaygroundShip
{
    private readonly HashSet<(int Row, int Column)> _livePositions = new();
    private readonly HashSet<(int Row, int Column)> _deadPositions = new();

    public PlaygroundShip(int row, int column, IShip ship, Orientation orientation)
    {
        // allows to create PlaygroundShip from IShip that is empty
        // TODO precheck that placement positions are always 0...x (this is only to validate that IShip follows its assumption)
        foreach (var (rowPosition, columnPosition) in ship.PlacementPositions())
        {
            var (_, height) = ship.OccupiedArea;
            var relativeRowPosition = rowPosition;
            var relativeColumnPosition = columnPosition;
            // IShip's PlacementPositions returns always in horizontal orientation, so we rotate only when orientation is Vertical
            if (orientation == Orientation.Vertical)
            {
                var tmpRow = relativeRowPosition;
                relativeRowPosition = relativeColumnPosition;
                relativeColumnPosition = height - tmpRow - 1;
            }
            _livePositions.Add((row + relativeRowPosition, column + relativeColumnPosition));
        }
    }

    public ActionResult Bomb(int row, int column)
    {
        var position = (row, column);
        var successfulHit = _livePositions.Remove(position);
        if (successfulHit)
        {
            _deadPositions.Add(position);
            return _livePositions.Any() ? ActionResult.Hit : ActionResult.Sink;
        }

        return _deadPositions.Contains(position) ? ActionResult.Hit : ActionResult.Miss;
    }

    public IEnumerable<(int Row, int Column)> LivePositions => _livePositions;

    public bool CheckIfShipFits(int rowsSize, int columnsSize)
    {
        foreach (var livePosition in _livePositions)
        {
            if (!livePosition.Row.InRange(0, rowsSize - 1) ||
                !livePosition.Column.InRange(0, columnsSize - 1))
                return false;
        }

        return true;
    }
}