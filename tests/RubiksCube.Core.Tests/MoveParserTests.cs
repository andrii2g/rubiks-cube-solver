using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class MoveParserTests
{
    [Fact]
    public void ParseSequence_ParsesBasicSingmasterNotation()
    {
        var result = MoveParser.ParseSequence("R U R' U' F2");

        Assert.True(result.IsSuccess);
        Assert.Equal([Move.R, Move.U, Move.Ri, Move.Ui, Move.F2], result.Value!.Moves);
    }

    [Fact]
    public void ParseSequence_ParsesInverseAliases()
    {
        var result = MoveParser.ParseSequence("Ui Ri Fi Di Li Bi");

        Assert.True(result.IsSuccess);
        Assert.Equal([Move.Ui, Move.Ri, Move.Fi, Move.Di, Move.Li, Move.Bi], result.Value!.Moves);
    }

    [Fact]
    public void ParseSequence_RejectsWideMovesInMvp()
    {
        var result = MoveParser.ParseSequence("R Rw U");

        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Equal("UnsupportedMoveToken", error.Code);
        Assert.Equal("Rw", error.Token);
        Assert.Equal(2, error.Position);
    }
}
