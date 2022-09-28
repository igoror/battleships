namespace BattleShips.Core.UI;

public interface IPlaygroundSideViewModel
{
    void SetCellStatus(CellStatus cellStatus, int row, int column);
    CellStatus[,] Board { get; }
    bool IsHumanBeing { get; }
    bool IsVisible { get; }
} 

// Left untested due to the same reason as Playground class 
public class PlaygroundSideSideViewModel : IPlaygroundSideViewModel
{
    private readonly bool _suppressLiveShipsParts;
    public CellStatus[,] Board { get; }
    public bool IsHumanBeing { get; }
    public bool IsVisible { get; }

    public PlaygroundSideSideViewModel(int rows, int columns, bool isHumanBeing, bool suppressLiveShipsParts,
        bool isVisible = true)
    {
        _suppressLiveShipsParts = suppressLiveShipsParts;
        IsHumanBeing = isHumanBeing;
        IsVisible = isVisible;
        Board = new CellStatus[rows, columns];
    }
    
    public void SetCellStatus(CellStatus cellStatus, int row, int column)
    {
        if (_suppressLiveShipsParts && cellStatus == CellStatus.ShipPart)
            return;
        
        Board[row, column] = cellStatus;
    }
}