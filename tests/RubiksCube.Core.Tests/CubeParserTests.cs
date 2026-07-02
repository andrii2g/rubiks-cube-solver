using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class CubeParserTests
{
    [Fact]
    public void ParseFaceletState_ParsesSolvedState()
    {
        var result = CubeParser.ParseFaceletState(Cube.SolvedState);

        Assert.True(result.IsSuccess);
        Assert.Equal(Cube.Solved, result.Value);
    }

    [Fact]
    public void ParseFaceletState_RejectsWrongLength()
    {
        var result = CubeParser.ParseFaceletState("UUU");

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "InvalidLength");
    }

    [Fact]
    public void ParseFaceletState_RejectsInvalidCounts()
    {
        var invalid = Cube.SolvedState.Replace('B', 'U');

        var result = CubeParser.ParseFaceletState(invalid);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "InvalidFaceletCount");
    }

    [Fact]
    public void ParseFaceletState_RejectsInvalidCenters()
    {
        var facelets = Cube.SolvedState.ToCharArray();
        (facelets[4], facelets[13]) = (facelets[13], facelets[4]);

        var result = CubeParser.ParseFaceletState(new string(facelets));

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.Code == "InvalidCenter" && error.Index == 4);
    }
}
