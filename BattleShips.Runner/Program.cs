// See https://aka.ms/new-console-template for more information

using BattleShips.Core.GameModel;
using BattleShips.Core.Players;
using BattleShips.Core.Ships;
using BattleShips.Core.UI;

var alivePlayer = new HumanBeingPlayer();
var randomPlayer = new AutonomousRandomPlayer();
// List of ships available during the gameplay1
var availableShips = new List<IShip>
{
    new DestroyerShip(), new DestroyerShip(), new BattleShip(), new BattleShip()
};
const int rows = 10;
const int columns = 10;

// With suppressLiveShipParts == false we can show full board
var firstPlayerSideViewModel = new PlaygroundSideSideViewModel(rows, columns, isHumanBeing:true, suppressLiveShipsParts: false);
var secondPlayerSideViewModel = new PlaygroundSideSideViewModel(rows, columns, isHumanBeing:false, suppressLiveShipsParts: true);
var consolePlaygroundVisualizer = new ConsolePlaygroundVisualizer(firstPlayerSideViewModel, secondPlayerSideViewModel);

var playgroundShipFactory = new PlaygroundShipFactory();
var playgroundSideFactory = new PlaygroundSideFactory(playgroundShipFactory);

var playground = new Playground(rows, columns, alivePlayer, randomPlayer, availableShips,
    consolePlaygroundVisualizer, playgroundSideFactory);
playground.InitializeFirstPlayer();
playground.InitializeSecondPlayer();
playground.Run();