using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class MoveSequenceTests
{
    [Fact]
    public void Inverse_ReversesAndInvertsMoves()
    {
        var sequence = new MoveSequence([Move.R, Move.U, Move.Ri, Move.Ui]);

        Assert.Equal("U R U' R'", sequence.Inverse().ToString());
    }

    [Theory]
    [InlineData("R R", "R2")]
    [InlineData("R R R", "R'")]
    [InlineData("R R R R", "")]
    [InlineData("R R'", "")]
    [InlineData("U2 U2", "")]
    public void Normalize_CollapsesAdjacentMovesOnSameFace(string input, string expected)
    {
        var sequence = MoveParser.ParseSequence(input).Value!;

        Assert.Equal(expected, sequence.Normalize().ToString());
    }
}
