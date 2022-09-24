using System;
using System.Collections.Generic;
using BattleShips.Core.GameModel;
using BattleShips.Core.Ships;
using FluentAssertions;
using NSubstitute;
using TddXt.AnyRoot.Numbers;
using Xunit;

namespace BattleShips.Core.UnitTests.GameModel;

public class PlaygroundSideTests
{
    private readonly IPlaygroundShipFactory _shipFactory = Substitute.For<IPlaygroundShipFactory>();
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 1)]
    [InlineData(1, -1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(-1, 11)]
    public void PlaygroundSide_WhenNonPositiveRowsOrColumns_ThrowsArgumentException(int rows, int columns)
    {
        var constructor = () => new PlaygroundSide(rows, columns, Substitute.For<IPlaygroundShipFactory>());

        constructor.Should().Throw<ArgumentException>()
            .WithMessage($"PlaygroundSide's rows and columns have to be positive. Provided (r: {rows}, c:{columns})");
    }
    
    [Fact]
    public void HasAliveShips_EmptySide_ReturnsFalse()
    {
        var playgroundSide = new PlaygroundSide(Any.Integer().Positive(), Any.Integer().Positive(),
            Substitute.For<IPlaygroundShipFactory>());

        playgroundSide.HasAliveShips().Should().BeFalse();
    }

    [Fact]
    public void PlaceShip_WhenShipDoesntFitInPlaygroundSide_ReturnsNull()
    {
        var shipRow = Any.Integer().Positive();
        var shipColumn = Any.Integer().Positive();
        var orientation = Any.Instance<Orientation>();
        var playgroundShip = Substitute.For<IPlaygroundShip>();
        playgroundShip.CheckIfShipFits(default, default).ReturnsForAnyArgs(false);
        var iShip = Substitute.For<IShip>();
        _shipFactory.Create(iShip, shipRow, shipColumn, orientation).Returns(playgroundShip);
        var playgroundSide = new PlaygroundSide(Any.Integer().Positive(), Any.Integer().Positive(), _shipFactory);

        var ship = playgroundSide.PlaceShip(iShip, shipRow, shipColumn, orientation);

        ship.Should().BeNull();
    }
    
    [Fact]
    public void PlaceShip_WhenShipIsEmpty_ReturnsNull()
    {
        var shipRow = Any.Integer().Positive();
        var shipColumn = Any.Integer().Positive();
        var orientation = Any.Instance<Orientation>();
        var playgroundShip = Substitute.For<IPlaygroundShip>();
        playgroundShip.CheckIfShipFits(default, default).ReturnsForAnyArgs(true);
        var iShip = Substitute.For<IShip>();
        _shipFactory.Create(iShip, shipRow, shipColumn, orientation).Returns(playgroundShip);
        var playgroundSide = new PlaygroundSide(Any.Integer().Positive(), Any.Integer().Positive(), _shipFactory);

        var ship = playgroundSide.PlaceShip(iShip, shipRow, shipColumn, orientation);

        ship.Should().BeNull();
    }
    
    [Fact]
    public void PlaceShip_WhenShipsOverlaps_ReturnsNull()
    {
        var shipRow = Any.Integer().Positive();
        var shipColumn = Any.Integer().Positive();
        var orientation = Any.Instance<Orientation>();
        var playgroundShip = Substitute.For<IPlaygroundShip>();
        playgroundShip.CheckIfShipFits(default, default).ReturnsForAnyArgs(true);
        playgroundShip.LivePositions.Returns(new List<(int, int)> {(1, 1)});
        var iShip = Substitute.For<IShip>();
        _shipFactory.Create(iShip, shipRow, shipColumn, orientation).Returns(playgroundShip);
        var playgroundSide = new PlaygroundSide(Any.Integer().Positive(), Any.Integer().Positive(), _shipFactory);

        var ship1 = playgroundSide.PlaceShip(iShip, shipRow, shipColumn, orientation);
        playgroundSide.HasAliveShips().Should().BeTrue();
        
        var ship2 = playgroundSide.PlaceShip(iShip, shipRow, shipColumn, orientation);

        ship1.Should().Be(playgroundShip);
        ship2.Should().BeNull();
        playgroundSide.HasAliveShips().Should().BeTrue();
    }

    [Fact]
    public void Bomb_DependingOnWhetherIsHitOrNot_ReturnsCorrectsActionResult()
    {
        var iShip = new DestroyerShip();
        var orientation = Any.Instance<Orientation>();
        var playgroundShip = new PlaygroundShip(0, 0, iShip, Orientation.Horizontal);
        _shipFactory.Create(iShip, 0, 0, orientation).Returns(playgroundShip);
        var playgroundSide = new PlaygroundSide(10, 10, _shipFactory);
        playgroundSide.PlaceShip(iShip, 0, 0, orientation);

        playgroundSide.Bomb(0, 0).Should().Be(ActionResult.Hit);
        playgroundSide.HasAliveShips().Should().BeTrue();
        playgroundSide.Bomb(1, 0).Should().Be(ActionResult.Miss);
        playgroundSide.HasAliveShips().Should().BeTrue();
        playgroundSide.Bomb(0, 1).Should().Be(ActionResult.Hit);
        playgroundSide.HasAliveShips().Should().BeTrue();
        playgroundSide.Bomb(0, 2).Should().Be(ActionResult.Hit);
        playgroundSide.HasAliveShips().Should().BeTrue();
        playgroundSide.Bomb(0, 3).Should().Be(ActionResult.Sink);
        playgroundSide.HasAliveShips().Should().BeFalse();
    }
}