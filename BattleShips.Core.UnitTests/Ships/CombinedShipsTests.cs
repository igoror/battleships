using System.Collections.Generic;
using BattleShips.Core.Ships;
using FluentAssertions;
using Xunit;

namespace BattleShips.Core.UnitTests.Ships;

public class CombinedShipsTests
{
    [Theory]
    [MemberData(nameof(ShipsTestData))]
    public void Ships_HasHorizontalPlacementPositions(IShip ship, (int, int) expectedOccupiedArea, 
        List<(int, int)> expectedPlacementPositions)
    {
        ship.OccupiedArea.Should().Be(expectedOccupiedArea);
        ship.PlacementPositions().Should().BeEquivalentTo(expectedPlacementPositions);
    }
    
    public static IEnumerable<object[]> ShipsTestData =>
        new List<object[]>
        {
            new object[] { new DestroyerShip(), (4,1), new List<(int, int)>{(0, 0), (0, 1), (0, 2), (0, 3)} },
            new object[] { new BattleShip(), (5,1), new List<(int, int)>{(0, 0), (0, 1), (0, 2), (0, 3), (0,4)} },
        };
}