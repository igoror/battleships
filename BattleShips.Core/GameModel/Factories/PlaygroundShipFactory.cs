using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

public interface IPlaygroundShipFactory
{
    IPlaygroundShip Create(IShip ship, int row, int column, Orientation orientation);
}

public class PlaygroundShipFactory : IPlaygroundShipFactory
{
    public IPlaygroundShip Create(IShip ship, int row, int column, Orientation orientation) =>
        new PlaygroundShip(row, column, ship, orientation);
}