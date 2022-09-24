using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

// Defines hooks on few different actions during the game
public interface IGameObserver
{
    void OnPlayerMove(PlayerType playerType);
    void OnPlayerMoved(PlayerType playerType, int row, int column, ActionResult actionResult);
    void OnShipPlaced(PlayerType playerType, IPlaygroundShip ship);
    void OnGameEnd(PlayerType winner);
    void OnGameEndWithError(string message);
    void OnShipPlacement(PlayerType playerType, IShip ship);
    void OnGameInitialization(int rows, int columns, IEnumerable<IShip> availableShips);
}