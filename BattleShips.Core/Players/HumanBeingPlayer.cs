using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;

namespace BattleShips.Core.Players;

public class HumanBeingPlayer : IPlayer
{
    public bool IsActiveBomber { get; }
    public bool IsActiveShipPlacer { get; }

    public HumanBeingPlayer(bool isActiveBomber = true, bool isActiveShipPlacer = true)
    {
        IsActiveBomber = isActiveBomber;
        IsActiveShipPlacer = isActiveShipPlacer;
    }
    
    public void InitializePlayerContext(int rows, int columns, IEnumerable<IShip> availableShips)
    {
        // Nothing to do for real player
    }

    public (int Row, int Column, Orientation rotation) DeclareShipPlacement(IShip ship)
    {
        var tokens = Console.ReadLine().Split(' ');
        if (tokens.Length != 2 // Check for 'rowCOLUMN ORIENTATION' 
            || tokens[0].Length < 2 // Check for 'rowCOLUMN' to have at least two chars
            || !tokens[0][0].IsLetter()) // Check for first sign to be upper letter
            return Constants.FailureShipPlacement;
        
        var row = Utils.ParseRow(tokens[0][0]);
        var columnParsed = int.TryParse(tokens[0][1..], out var column);
        
        // We accept only A-Z as row id, that's why we enforce MAX_DIMENSION_SIZE
        if (!columnParsed || !RowColumnValidation(row, column))
            return Constants.FailureShipPlacement;
        // Checking if we are able to parse enum
        if (!Enum.TryParse<Orientation>(tokens[1], out var rotation))
            return Constants.FailureShipPlacement;
        
        return (row, column, rotation);
    }

    public (int Row, int Column) NextMove()
    {
        var input = Console.ReadLine();
        if (input.Length < 2 // Check for 'rowCOLUMN' to have at least two chars
            || !input[0].IsLetter()) // Check for first sign to be upper letter
            return Constants.FailureBombingPosition;
        
        var row = Utils.ParseRow(input[0]);
        var columnParsed = int.TryParse(input[1..], out var column);
        return !columnParsed || !RowColumnValidation(row, column) ? Constants.FailureBombingPosition : (row, column);
    }

    private static bool RowColumnValidation(int row, int column)
    {
        return !(row > Constants.MaxDimensionSizeWithHumanAsPlayer
               || column > Constants.MaxDimensionSizeWithHumanAsPlayer);
    }
    

    public void StoreLastMoveResult(ActionResult result)
    {
        // Nothing to do for real player
    }
}