using BattleShips.Core.Players;
using BattleShips.Core.Ships;

namespace BattleShips.Core.GameModel;

public enum ActionResult
{
    Miss,
    Hit,
    Sink
}

public interface IPlayground
{
    void InitializeFirstPlayer();
    void InitializeSecondPlayer();
    PlayerType Run();
}

// I left this class untested because I'm reaching one work day mark and don't want to exceed the time spent on that task
// so that you have precise info on how much I did during that time.
public class Playground : IPlayground
{
    private readonly int _rows;
    private readonly int _columns;
    private readonly IPlayer _player1;
    private readonly IPlayer _player2;
    private readonly IEnumerable<IShip> _availableShips;
    private readonly IGameObserver _gameObserver;
    private readonly IPlaygroundSide _player1Side;
    private readonly IPlaygroundSide _player2Side;
    private const int RetryCount = 3;
    private bool _firstPlayerInitialized;
    private bool _secondPlayerInitialized;
    
    public Playground(int rows, int columns, IPlayer player1, IPlayer player2, IEnumerable<IShip> availableShips,
        IGameObserver gameObserver, IPlaygroundSideFactory playgroundSideFactory)
    {
        if (rows <= 0 || columns <= 0)
            throw new ArgumentException("Playground should have positive number of rows and columns");
        
        _rows = rows;
        _columns = columns;
        _player1 = player1;
        _player2 = player2;
        _availableShips = availableShips;
        _gameObserver = gameObserver;
        _player1Side = playgroundSideFactory.Create(rows, columns);
        _player2Side = playgroundSideFactory.Create(rows, columns);
        _gameObserver.OnGameInitialization(rows, columns, _availableShips);
    }

    public void InitializeFirstPlayer()
    {
        InitializePlayer(PlayerType.First);
        _firstPlayerInitialized = true;
    } 

    public void InitializeSecondPlayer()
    {
        InitializePlayer(PlayerType.Second);
        _secondPlayerInitialized = true;
    } 

    private void InitializePlayer(PlayerType playerType)
    {
        var player = playerType == PlayerType.First ? _player1 : _player2;
        var playgroundSide = playerType == PlayerType.First ? _player1Side : _player2Side; 
        player.InitializePlayerContext(_rows, _columns, _availableShips);
        foreach (var ship in _availableShips)
        {
            IPlaygroundShip? playgroundShip = null;
            for (var retryCount = 0; retryCount < RetryCount; ++retryCount)
            {
                _gameObserver.OnShipPlacement(playerType, ship);
                var (row, column, rotation) = player.DeclareShipPlacement(ship);
                playgroundShip = playgroundSide.PlaceShip(ship, row, column, rotation);
                if (playgroundShip != null)
                {
                    _gameObserver.OnShipPlaced(playerType, playgroundShip);
                    break;
                }
            }

            if (playgroundShip == null)
            {
                var errorMessage = $"Player:'{playerType}' was unable to correctly place a ship for {RetryCount} times in a row";
                _gameObserver.OnGameEndWithError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
    
    public PlayerType Run()
    {
        if (!_firstPlayerInitialized || !_secondPlayerInitialized)
            throw new InvalidOperationException("Both players must be initialized before running game");
        
        var oppositePlayerBoard = _player2Side;
        var currentPlayer = _player1;
        var currentPlayerType = PlayerType.First;
        while (true)
        {
            var movedSuccessfully = false;
            for (var retryCount = 0; retryCount < RetryCount; ++retryCount)
            {
                _gameObserver.OnPlayerMove(currentPlayerType);
                var nextMove =  currentPlayer.NextMove();
                if (nextMove == Constants.FailureBombingPosition 
                    || !nextMove.Row.InRange(0, _rows - 1) || !nextMove.Column.InRange(0, _columns - 1) )
                    continue;
                
                var moveResult = oppositePlayerBoard.Bomb(nextMove.Row, nextMove.Column);
                currentPlayer.StoreLastMoveResult(moveResult);
                _gameObserver.OnPlayerMoved(currentPlayerType, nextMove.Row, nextMove.Column, moveResult);
                movedSuccessfully = true;
                break;
            }

            if (!movedSuccessfully)
            {
                var errorMessage = $"Player:{currentPlayerType} was unable to place bomb for {RetryCount} times in a row.";
                _gameObserver.OnGameEndWithError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (oppositePlayerBoard.HasAliveShips())
            {
                currentPlayer = currentPlayerType == PlayerType.First ? _player2 : _player1;
                oppositePlayerBoard = currentPlayerType == PlayerType.First ? _player1Side : _player2Side;
                currentPlayerType = currentPlayerType.Other();
            }
            else
                break;
        }
        
        _gameObserver.OnGameEnd(currentPlayerType);
        return currentPlayerType;
    }
}