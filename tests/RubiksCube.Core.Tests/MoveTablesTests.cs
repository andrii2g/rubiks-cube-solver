using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class MoveTablesTests
{
    [Theory]
    [InlineData(Face.U)]
    [InlineData(Face.R)]
    [InlineData(Face.F)]
    [InlineData(Face.D)]
    [InlineData(Face.L)]
    [InlineData(Face.B)]
    public void GetClockwisePermutation_ReturnsValidPermutation(Face face)
    {
        var permutation = MoveTables.GetClockwisePermutation(face).ToArray();

        Assert.Equal(Cube.StickerCount, permutation.Length);
        Assert.Equal(Enumerable.Range(0, Cube.StickerCount), permutation.Order());
    }
}
