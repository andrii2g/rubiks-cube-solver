using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class CubeMoveTests
{
    public static TheoryData<Move> AllBaseMoves => new()
    {
        Move.U,
        Move.R,
        Move.F,
        Move.D,
        Move.L,
        Move.B
    };

    [Theory]
    [MemberData(nameof(AllBaseMoves))]
    public void Apply_FourClockwiseTurns_ReturnsOriginalCube(Move move)
    {
        var cube = Cube.Solved.Apply(move).Apply(move).Apply(move).Apply(move);

        Assert.Equal(Cube.Solved, cube);
    }

    [Theory]
    [MemberData(nameof(AllBaseMoves))]
    public void Apply_MoveAndInverse_ReturnsOriginalCube(Move move)
    {
        var cube = Cube.Solved.Apply(move).Apply(move.Inverse());

        Assert.Equal(Cube.Solved, cube);
    }

    [Theory]
    [MemberData(nameof(AllBaseMoves))]
    public void Apply_HalfTurnTwice_ReturnsOriginalCube(Move move)
    {
        var halfTurn = new Move(move.Face, TurnAmount.Half);

        var cube = Cube.Solved.Apply(halfTurn).Apply(halfTurn);

        Assert.Equal(Cube.Solved, cube);
    }

    [Fact]
    public void Apply_ScramblePlusInverse_SolvesCube()
    {
        var scramble = MoveParser.ParseSequence("R U R' U' F2").Value!;

        var cube = Cube.Solved.Apply(scramble.Moves).Apply(scramble.Inverse().Moves);

        Assert.Equal(Cube.Solved, cube);
    }

    [Fact]
    public void Apply_RandomScramble_PreservesStickerCounts()
    {
        var scramble = ScrambleGenerator.Generate(25, seed: 123);

        var state = Cube.Solved.Apply(scramble.Moves).ToFaceletString();

        foreach (var facelet in "URFDLB")
        {
            Assert.Equal(9, state.Count(value => value == facelet));
        }
    }
}
