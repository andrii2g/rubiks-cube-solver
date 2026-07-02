using RubiksCube.Cli;
using RubiksCube.Core;

namespace RubiksCube.Cli.Tests;

public sealed class RubikCliTests
{
    [Fact]
    public void SolveScramble_ReturnsVerifiedSolution()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var exitCode = RubikCli.Run(["solve", "--scramble", "R U R' U'"], output, error);

        Assert.Equal(0, exitCode);
        Assert.Contains("Status:     Solved", output.ToString());
        Assert.Contains("Verified:   yes", output.ToString());
    }

    [Fact]
    public void ValidateSolvedState_ReturnsValid()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();

        var exitCode = RubikCli.Run(["validate", "--state", Cube.SolvedState], output, error);

        Assert.Equal(0, exitCode);
        Assert.Contains("Valid: yes", output.ToString());
    }

    [Fact]
    public void Random_WithSeed_IsDeterministic()
    {
        using var firstOutput = new StringWriter();
        using var secondOutput = new StringWriter();
        using var error = new StringWriter();

        RubikCli.Run(["random", "--length", "10", "--seed", "123"], firstOutput, error);
        RubikCli.Run(["random", "--length", "10", "--seed", "123"], secondOutput, error);

        Assert.Equal(firstOutput.ToString(), secondOutput.ToString());
    }

    [Fact]
    public void SolveState_ForUnsolvedState_ReturnsSolverUnavailable()
    {
        using var output = new StringWriter();
        using var error = new StringWriter();
        var state = Cube.Solved.Apply(Move.R).ToFaceletString();

        var exitCode = RubikCli.Run(["solve", "--state", state], output, error);

        Assert.Equal(2, exitCode);
        Assert.Contains("SolverUnavailable", output.ToString());
    }
}
