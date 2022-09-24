namespace BattleShips.Core.Ships;

public interface IShip
{
    // Returns relative, 0 indexed placement positions of the ship. 
    // This should return placement positions as the ship was placed horizontally.
    IEnumerable<(int Row, int Column)> PlacementPositions();
    
    // Returns smallest rectangle which the ships fits in.
    // Example: (S - for ship part, E - for empty) 
    // SSEE
    // ESSS <-- this Z ship's occupied area is (4, 2)
    
    // SSSS <-- (4, 1)
    (int Width, int Height) OccupiedArea { get; }
}