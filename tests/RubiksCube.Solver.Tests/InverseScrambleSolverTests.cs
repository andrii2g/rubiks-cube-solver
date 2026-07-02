using RubiksCube.Core;
using RubiksCube.Solver;

namespace RubiksCube.Solver.Tests;

public sealed class InverseScrambleSolverTests
{
    [Fact]
    public void Solve_EmptyScramble_ReturnsSolved()
    {
        var result = new InverseScrambleSolver().Solve(new SolveRequest(SolveInputType.Scramble, ""));

        Assert.Equal(SolveStatus.Solved, result.Status);
        Assert.Empty(result.Moves);
        Assert.True(result.Verified);
    }

    [Fact]
    public void Solve_SingleMove_ReturnsInverseMove()
    {
        var result = new InverseScrambleSolver().Solve(new SolveRequest(SolveInputType.Scramble, "R"));

        Assert.Equal(SolveStatus.Solved, result.Status);
        Assert.Equal([Move.Ri], result.Moves);
        Assert.True(result.Verified);
    }

    [Fact]
    public void Solve_RandomScrambles_VerifiesSolvedState()
    {
        var solver = new InverseScrambleSolver();

        for (var seed = 0; seed < 100; seed++)
        {
            var scramble = ScrambleGenerator.Generate(25, seed).ToString();
            var result = solver.Solve(new SolveRequest(SolveInputType.Scramble, scramble));

            Assert.Equal(SolveStatus.Solved, result.Status);
            Assert.True(result.Verified);
        }
    }

    [Fact]
    public void Solve_InvalidInput_ReturnsInvalidInput()
    {
        var result = new InverseScrambleSolver().Solve(new SolveRequest(SolveInputType.Scramble, "R Rw"));

        Assert.Equal(SolveStatus.InvalidInput, result.Status);
        Assert.Contains("Wide moves", result.Message);
    }
}
