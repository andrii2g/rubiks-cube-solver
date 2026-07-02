using System.Diagnostics;
using RubiksCube.Core;

namespace RubiksCube.Solver;

public sealed class InverseScrambleSolver : ICubeSolver
{
    public SolverId Id => SolverId.InverseScramble;

    public bool CanSolve(SolveRequest request) =>
        request.InputType == SolveInputType.Scramble &&
        request.SolverId is SolverId.Auto or SolverId.InverseScramble;

    public SolveResult Solve(SolveRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!CanSolve(request))
        {
            return Failure(SolveStatus.SolverUnavailable, "Inverse scramble solver only supports scramble input.", stopwatch.Elapsed);
        }

        var parseResult = MoveParser.ParseSequence(request.Input);
        if (!parseResult.IsSuccess)
        {
            return Failure(SolveStatus.InvalidInput, string.Join(Environment.NewLine, parseResult.Errors.Select(error => error.Message)), stopwatch.Elapsed);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var scramble = parseResult.Value!;
        var initialState = Cube.Solved.Apply(scramble.Moves);
        var solution = scramble.Inverse().Normalize();
        var steps = request.IncludeSnapshots
            ? CreateSteps(initialState, solution.Moves, cancellationToken)
            : Array.Empty<SolveStep>();

        var verified = VerificationSolver.Verify(initialState, solution.Moves, out var finalState);
        stopwatch.Stop();

        return verified
            ? new SolveResult(SolveStatus.Solved, solution.Moves, steps, initialState, finalState, null, stopwatch.Elapsed)
            : new SolveResult(
                SolveStatus.InternalError,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                initialState,
                finalState,
                "Solver verification failed.",
                stopwatch.Elapsed);
    }

    private static IReadOnlyList<SolveStep> CreateSteps(Cube initialState, IReadOnlyList<Move> moves, CancellationToken cancellationToken)
    {
        var steps = new List<SolveStep>(moves.Count);
        var current = initialState;

        for (var i = 0; i < moves.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var before = current;
            current = current.Apply(moves[i]);
            steps.Add(new SolveStep(
                i + 1,
                moves[i],
                $"Apply {moves[i]}",
                SolverPhase.InverseScramble,
                before,
                current));
        }

        return steps;
    }

    private static SolveResult Failure(SolveStatus status, string message, TimeSpan elapsed) =>
        new(status, Array.Empty<Move>(), Array.Empty<SolveStep>(), null, null, message, elapsed);
}
