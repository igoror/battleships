using System.Collections.Generic;
using BattleShips.Core.GameModel;
using BattleShips.Core.Players;
using BattleShips.Core.Ships;
using NSubstitute;
using Xunit;

namespace BattleShips.Core.IntegrationTests;

public class GameTests
{
    [Fact]
    public void ComputersAgainstEachOther_GameShouldAlwaysFinish()
    {
        for (var i = 0; i < 10000; ++i)
        {
            SingleRun();
        }
    }

    private static void SingleRun()
    {
        var randomPlayer1 = new AutonomousRandomPlayer();
        var randomPlayer2 = new AutonomousRandomPlayer();
        var availableShips = new List<IShip>
        {
            new DestroyerShip(), new DestroyerShip(), new BattleShip(), new BattleShip()
        };
        const int rows = 10;
        const int columns = 10;

        var playgroundShipFactory = new PlaygroundShipFactory();
        var playgroundSideFactory = new PlaygroundSideFactory(playgroundShipFactory);

        var playground = new Playground(rows, columns, randomPlayer1, randomPlayer2, availableShips,
            Substitute.For<IGameObserver>(), playgroundSideFactory);
        playground.InitializeFirstPlayer();
        playground.InitializeSecondPlayer();
        playground.Run();
    }
}