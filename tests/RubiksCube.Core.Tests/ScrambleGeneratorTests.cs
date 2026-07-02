using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class ScrambleGeneratorTests
{
    [Fact]
    public void Generate_WithSameSeed_ReturnsSameScramble()
    {
        var first = ScrambleGenerator.Generate(25, seed: 123);
        var second = ScrambleGenerator.Generate(25, seed: 123);

        Assert.Equal(first.ToString(), second.ToString());
    }

    [Fact]
    public void Generate_DoesNotRepeatAdjacentFaces()
    {
        var scramble = ScrambleGenerator.Generate(100, seed: 123);

        for (var i = 1; i < scramble.Moves.Count; i++)
        {
            Assert.NotEqual(scramble.Moves[i - 1].Face, scramble.Moves[i].Face);
        }
    }
}
