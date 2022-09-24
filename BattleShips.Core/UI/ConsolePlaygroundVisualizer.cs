using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;

namespace BattleShips.Core.UI;


public class ConsolePlaygroundVisualizer : IGameObserver
{
    private readonly IPlaygroundSideViewModel _firstPlayerSideSideViewModel;
    private readonly IPlaygroundSideViewModel _secondPlayerSideSideViewModel;
    private int _rows;
    private int _columns;
    private IEnumerable<IShip> _availableShips;

    public ConsolePlaygroundVisualizer(IPlaygroundSideViewModel firstPlayerSideSideViewModel, 
        IPlaygroundSideViewModel secondPlayerSideSideViewModel)
    {
        _firstPlayerSideSideViewModel = firstPlayerSideSideViewModel;
        _secondPlayerSideSideViewModel = secondPlayerSideSideViewModel;
        Refresh();
    }

    public void OnPlayerMove(PlayerType playerType)
    {
        var viewModel = GetViewModel(playerType);
        if (!viewModel.IsHumanBeing)
            return;
        
        Console.WriteLine($"Insert next bombing position. Format 'rowCOLUMN'");
    }

    public void OnPlayerMoved(PlayerType playerType, int row, int column, ActionResult actionResult)
    {
        var viewModel = GetViewModel(playerType.Other());
        var newCellStatus = actionResult switch
        {
            ActionResult.Hit => CellStatus.DestroyedShipPart,
            ActionResult.Miss => CellStatus.Missed,
            ActionResult.Sink => CellStatus.DestroyedShipPart,
            _ => CellStatus.Empty
        };
        viewModel.SetCellStatus(newCellStatus, row, column);
        Refresh();
    }

    public void OnShipPlaced(PlayerType playerType, IPlaygroundShip ship)
    {
        var viewModel = GetViewModel(playerType);
        foreach (var (row, column) in ship.LivePositions)
        {
            viewModel.SetCellStatus(CellStatus.ShipPart, row, column);
        }
        Refresh();
    }

    private IPlaygroundSideViewModel GetViewModel(PlayerType playerType) => playerType == PlayerType.First
        ? _firstPlayerSideSideViewModel
        : _secondPlayerSideSideViewModel;

    private void Refresh()
    {
        Console.Clear();
        PrintPlayer(PlayerType.Second);
        PrintPlayer(PlayerType.First);
    }

    private void PrintPlayer(PlayerType playerType)
    {
        var viewModel = GetViewModel(playerType);
        Console.WriteLine($"Player: {playerType}");
        for (var row = 0; row < viewModel.Board.GetLength(0); ++row)
        {
            for (var column = 0; column < viewModel.Board.GetLength(1); ++column)
                WriteCellStatus(viewModel.Board[row, column]);
            Console.WriteLine();
        }
    }
    
    private static void WriteCellStatus(CellStatus cellStatus)
    {
        switch (cellStatus)
        {
            case CellStatus.Empty:
                Console.Write("O");
                break;
            case CellStatus.Missed:
                WriteOnBackground("M", ConsoleColor.Blue);
                break;
            case CellStatus.ShipPart:
                WriteOnBackground("S", ConsoleColor.Green);
                break;
            case CellStatus.DestroyedShipPart:
                WriteOnBackground("S", ConsoleColor.Red);
                break;
        }
    }

    private static void WriteOnBackground(string msg, ConsoleColor color)
    {
        Console.BackgroundColor = color;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(msg);
        Console.ResetColor();
    }

    public void OnGameEnd(PlayerType winner)
    {
        Console.WriteLine($"Game finished. {winner} player won.");
    }

    public void OnGameEndWithError(string message)
    {
        Console.WriteLine($"Game ended due to error - {message}");
    }

    public void OnShipPlacement(PlayerType playerType, IShip ship)
    {
        var viewModel = GetViewModel(playerType);
        if (!viewModel.IsHumanBeing)
            return;
        
        Console.WriteLine($"Board size: (rows:{_rows}, columns:{_columns}. Available ships: {string.Join(", ", _availableShips.Select(x => x.GetType().Name))}");
        Console.WriteLine($"Insert placement for ship of type {ship.GetType().Name}. Format 'rowCOLUMN ROTATION'. " +
                          $"Available Rotations (0:Horizontal, 1: Vertical)");
    }

    public void OnGameInitialization(int rows, int columns, IEnumerable<IShip> availableShips)
    {
        _rows = rows;
        _columns = columns;
        _availableShips = availableShips;
    }
}