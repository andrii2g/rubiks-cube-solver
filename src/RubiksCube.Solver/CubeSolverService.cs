using System.Diagnostics;
using RubiksCube.Core;
using RubiksCube.Solver.TwoPhase;

namespace RubiksCube.Solver;

public sealed class CubeSolverService
{
    private readonly InverseScrambleSolver _inverseScrambleSolver = new();
    private readonly TwoPhaseSolver _twoPhaseSolver = new();

    public SolveResult Solve(SolveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.InputType == SolveInputType.Scramble)
        {
            return _inverseScrambleSolver.Solve(request, cancellationToken);
        }

        return _twoPhaseSolver.Solve(request, cancellationToken);
    }
}
