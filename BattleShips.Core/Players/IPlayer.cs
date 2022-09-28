using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;

namespace BattleShips.Core.Players;

// Defines actions that each player has to implement
public interface IPlayer
{
    // Informational method. Board size and available ships during the game provided as arguments  
    void InitializePlayerContext(int rows, int columns, IEnumerable<IShip> availableShips);
    // Return tuple points where particular type of ship should be placed
    (int Row, int Column, Orientation rotation) DeclareShipPlacement(IShip ship);
    // Return tuple defines where next bomb should be dropped
    (int Row, int Column) NextMove();
    // Informational method. Result of last move provided as argument.
    void StoreLastMoveResult(ActionResult result);
    bool IsActiveBomber { get; }
    bool IsActiveShipPlacer { get; }
}