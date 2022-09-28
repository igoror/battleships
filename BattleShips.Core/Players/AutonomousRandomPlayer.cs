using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;

namespace BattleShips.Core.Players;

public class AutonomousRandomPlayer : IPlayer
{
    public bool IsActiveBomber { get; }
    public bool IsActiveShipPlacer { get; }
    private readonly HashSet<(int Row, int Column)> _availableMoves = new();
    private readonly HashSet<int> _availableRows = new();
    private readonly Random _random = Random.Shared;
    private int _rows;
    private int _columns;
    
    public AutonomousRandomPlayer(bool isActiveBomber = true, bool isActiveShipPlacer = true)
    {
        IsActiveBomber = isActiveBomber;
        IsActiveShipPlacer = isActiveShipPlacer;
    }

    public void InitializePlayerContext(int rows, int columns, IEnumerable<IShip> availableShips)
    {
        _rows = rows;
        _columns = columns;
        for (var i = 0; i < rows; ++i)
        {
            _availableRows.Add(i);
            for (var j = 0; j < columns; ++j)
                _availableMoves.Add((i, j));
        }
    }
    
    public (int Row, int Column, Orientation rotation) DeclareShipPlacement(IShip ship)
    {
        var row = _availableRows.ElementAt(_random.Next(0, _availableRows.Count - 1));
        _availableRows.Remove(row);
        var column = ship switch
        {
            // -1 in order to fit in table, -3 because its length is 4
            DestroyerShip => _random.Next(0, _columns - 3 - 1),
            // -1 in order to fit in table, -3 because its length is 5
            BattleShip => _random.Next(0, _columns - 4 - 1),
            _ => throw new NotSupportedException($"{ship.GetType().Name} is not supported")
        };

        // placing all ships horizontally
        return (row, column, Orientation.Horizontal);
    }

    public (int Row, int Column) NextMove()
    {
        var nextMove = _availableMoves.ElementAt(_random.Next(0, _availableMoves.Count - 1));
        _availableMoves.Remove(nextMove);
        return nextMove;
    }

    public void StoreLastMoveResult(ActionResult result)
    {
        // Doesn't matter since this player is random 
    }
}