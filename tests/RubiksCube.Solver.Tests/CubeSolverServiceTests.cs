using RubiksCube.Core;
using RubiksCube.Solver;

namespace RubiksCube.Solver.Tests;

public sealed class CubeSolverServiceTests
{
    [Fact]
    public void Solve_SolvedFaceletState_ReturnsSolved()
    {
        var result = new CubeSolverService().Solve(new SolveRequest(SolveInputType.FaceletState, Cube.SolvedState));

        Assert.Equal(SolveStatus.Solved, result.Status);
        Assert.True(result.Verified);
    }

    [Fact]
    public void Solve_ValidUnsolvedFaceletState_ReturnsSolverUnavailable()
    {
        var state = Cube.Solved.Apply(Move.R).ToFaceletString();

        var result = new CubeSolverService().Solve(new SolveRequest(SolveInputType.FaceletState, state));

        Assert.Equal(SolveStatus.SolverUnavailable, result.Status);
    }

    [Fact]
    public void Solve_InvalidFaceletState_ReturnsInvalidState()
    {
        var result = new CubeSolverService().Solve(new SolveRequest(SolveInputType.FaceletState, "UUU"));

        Assert.Equal(SolveStatus.InvalidState, result.Status);
    }

    [Fact]
    public void Solve_ImpossibleFaceletState_ReturnsNotSolvable()
    {
        var facelets = Cube.SolvedState.ToCharArray();
        (facelets[7], facelets[19]) = (facelets[19], facelets[7]);

        var result = new CubeSolverService().Solve(new SolveRequest(SolveInputType.FaceletState, new string(facelets)));

        Assert.Equal(SolveStatus.NotSolvable, result.Status);
    }
}
