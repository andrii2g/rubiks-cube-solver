namespace RubiksCube.Solver;

public interface ICubeSolver
{
    SolverId Id { get; }

    bool CanSolve(SolveRequest request);

    SolveResult Solve(SolveRequest request, CancellationToken cancellationToken = default);
}
