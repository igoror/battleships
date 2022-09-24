using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

public interface IPlaygroundSideFactory
{
    PlaygroundSide Create(int rows, int columns);
}

public class PlaygroundSideFactory : IPlaygroundSideFactory
{
    private readonly IPlaygroundShipFactory _playgroundShipFactory;

    public PlaygroundSideFactory(IPlaygroundShipFactory playgroundShipFactory)
    {
        _playgroundShipFactory = playgroundShipFactory;
    }

    public PlaygroundSide Create(int rows, int columns) =>
        new PlaygroundSide(rows, columns, _playgroundShipFactory);
}