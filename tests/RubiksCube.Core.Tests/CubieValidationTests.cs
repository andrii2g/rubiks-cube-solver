using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class CubieValidationTests
{
    [Fact]
    public void ValidatePhysicalState_SolvedState_IsValid()
    {
        var result = CubeValidator.ValidatePhysicalState(Cube.SolvedState);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidatePhysicalState_ScrambleGeneratedState_IsValid()
    {
        var state = Cube.Solved.Apply(ScrambleGenerator.Generate(30, seed: 123).Moves).ToFaceletString();

        var result = CubeValidator.ValidatePhysicalState(state);

        Assert.True(result.IsValid, string.Join(Environment.NewLine, result.Errors.Select(error => error.Message)));
    }

    [Fact]
    public void ValidatePhysicalState_SingleFlippedEdge_IsInvalid()
    {
        var facelets = Cube.SolvedState.ToCharArray();
        (facelets[7], facelets[19]) = (facelets[19], facelets[7]);

        var result = CubeValidator.ValidatePhysicalState(new string(facelets));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Code == "InvalidEdgeOrientation");
    }

    [Fact]
    public void ValidatePhysicalState_SingleTwistedCorner_IsInvalid()
    {
        var facelets = Cube.SolvedState.ToCharArray();
        (facelets[8], facelets[9], facelets[20]) = (facelets[20], facelets[8], facelets[9]);

        var result = CubeValidator.ValidatePhysicalState(new string(facelets));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Code == "InvalidCornerOrientation");
    }

    [Fact]
    public void ValidatePhysicalState_SingleSwappedEdge_IsInvalid()
    {
        var facelets = Cube.SolvedState.ToCharArray();
        (facelets[10], facelets[19]) = (facelets[19], facelets[10]);

        var result = CubeValidator.ValidatePhysicalState(new string(facelets));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Code == "InvalidPermutationParity");
    }
}
