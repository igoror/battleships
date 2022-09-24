using System;

namespace BattleShips.Core.UnitTests;

public static class Extensions
{
    // This has bug for INT.MIN but this is for tests only ;) 
    public static int Positive(this int a) => Math.Abs(a) + 1;
}