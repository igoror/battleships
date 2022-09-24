global using static TddXt.AnyRoot.Root;
using System.Collections.Generic;
using System.Linq;
using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;
using FluentAssertions;
using NSubstitute;
using TddXt.AnyRoot.Numbers;
using Xunit;

namespace BattleShips.Core.UnitTests.GameModel;

public class PlaygroundShipTests
{
    [Fact]
    public void PlaygroundShip_HorizontalOrientation_MovesShip_AccordingToProvided_RowAndColumn()
    {
        var iShip = Substitute.For<IShip>();
        // SSE
        // ESS
        var iShipPositions = new List<(int, int)> {(0, 0), (0, 1), (1, 1), (1, 2)};
        iShip.PlacementPositions().Returns(iShipPositions);
        var placementRow = Any.Integer();
        var placementColumn = Any.Integer();

        var playgroundShip = new PlaygroundShip(placementRow, placementColumn, iShip, Orientation.Horizontal);

        var expectedLivePositions =
            iShipPositions.Select(pos => (pos.Item1 + placementRow, pos.Item2 + placementColumn));
        playgroundShip.LivePositions.Should().BeEquivalentTo(expectedLivePositions);
    }
    
    [Fact]
    public void PlaygroundShip_VerticalOrientation_MovesAndRotatesShip_AccordingToProvided_RowAndColumn()
    {
        var iShip = Substitute.For<IShip>();
        // SSE
        // ESS
        // After rotation
        // ES
        // SS
        // SE
        var iShipPositions = new List<(int, int)> { (0, 0), (0, 1), (1, 1), (1, 2)};
        iShip.PlacementPositions().Returns(iShipPositions);
        iShip.OccupiedArea.Returns((3, 2));
        var placementRow = 0; Any.Integer();
        var placementColumn = 0; Any.Integer();

        var playgroundShip = new PlaygroundShip(placementRow, placementColumn, iShip, Orientation.Vertical);

        var rotatedLivePositions = new List<(int, int)> { (0,1),(1,0),(1,1),(2,0) };
        var expectedLivePositions = 
            rotatedLivePositions.Select(pos => (pos.Item1 + placementRow, pos.Item2 + placementColumn));
        playgroundShip.LivePositions.Should().BeEquivalentTo(expectedLivePositions);
    }

    [Theory]
    [InlineData(1,0)]
    [InlineData(1,1)]
    [InlineData(0,3)]
    public void Bomb_WhenOutsideLivePositions_ReturnsMiss(int row, int column)
    {
        // SSS
        var ship = ThreeXOneShip(0, 0);

        ship.Bomb(row, column).Should().Be(ActionResult.Miss);
    }

    [Fact]
    public void Bomb_WhenHitAllShipParts_ReturnsSink()
    {
        // SSS
        var ship = ThreeXOneShip(0, 0);

        ship.Bomb(0, 0).Should().Be(ActionResult.Hit);
        ship.Bomb(0, 1).Should().Be(ActionResult.Hit);
        ship.Bomb(0, 2).Should().Be(ActionResult.Sink);
    }
    
    [Fact]
    public void Bomb_DoubleHitSamePart_ReturnsHit()
    {
        // SSS
        var ship = ThreeXOneShip(0, 0);

        ship.Bomb(0, 0).Should().Be(ActionResult.Hit);
        ship.Bomb(0, 0).Should().Be(ActionResult.Hit);
    }

    [Theory]
    [InlineData(1,2,false)]
    [InlineData(0,3,false)]
    [InlineData(1,3,true)]
    public void Validate_TestBoundariesValidation(int rowBoundary, int columnBoundary, bool expectedResult)
    {
        // SSS
        var ship = ThreeXOneShip(0, 0);

        ship.CheckIfShipFits(rowBoundary, columnBoundary).Should().Be(expectedResult);
    }
    
    private static PlaygroundShip ThreeXOneShip(int row, int column)
    {
        var iShip = Substitute.For<IShip>();
        iShip.OccupiedArea.Returns((3, 1));
        iShip.PlacementPositions().Returns(new List<(int Row, int Column)>{(0,0), (0,1), (0,2)});
        return new PlaygroundShip(row, column, iShip, Orientation.Horizontal);
    }
}