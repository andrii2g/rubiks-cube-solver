using System.Diagnostics;
using RubiksCube.Core;

namespace RubiksCube.Solver;

public sealed class CubeSolverService
{
    private readonly InverseScrambleSolver _inverseScrambleSolver = new();

    public SolveResult Solve(SolveRequest request, CancellationToken cancellationToken = default)
    {
        if (request.InputType == SolveInputType.Scramble)
        {
            return _inverseScrambleSolver.Solve(request, cancellationToken);
        }

        return SolveFaceletState(request);
    }

    private static SolveResult SolveFaceletState(SolveRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var parseResult = CubeParser.ParseFaceletState(request.Input);
        stopwatch.Stop();

        if (!parseResult.IsSuccess)
        {
            return new SolveResult(
                SolveStatus.InvalidState,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                null,
                null,
                string.Join(Environment.NewLine, parseResult.Errors.Select(error => error.Message)),
                stopwatch.Elapsed);
        }

        var physicalValidation = CubeValidator.ValidatePhysicalState(request.Input);
        if (!physicalValidation.IsValid)
        {
            stopwatch.Stop();
            return new SolveResult(
                SolveStatus.NotSolvable,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                parseResult.Value,
                parseResult.Value,
                string.Join(Environment.NewLine, physicalValidation.Errors.Select(error => error.Message)),
                stopwatch.Elapsed);
        }

        if (parseResult.Value!.IsSolved())
        {
            return new SolveResult(
                SolveStatus.Solved,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                parseResult.Value,
                parseResult.Value,
                null,
                stopwatch.Elapsed);
        }

        return new SolveResult(
            SolveStatus.SolverUnavailable,
            Array.Empty<Move>(),
            Array.Empty<SolveStep>(),
            parseResult.Value,
            parseResult.Value,
            "Arbitrary 54-facelet state solving requires the planned two-phase solver.",
            stopwatch.Elapsed);
    }
}
