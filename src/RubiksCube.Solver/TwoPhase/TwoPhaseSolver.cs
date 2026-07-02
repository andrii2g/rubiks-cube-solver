using System.Diagnostics;
using RubiksCube.Core;

namespace RubiksCube.Solver.TwoPhase;

public sealed class TwoPhaseSolver : ICubeSolver
{
    private const int DefaultMaxDepth = 10;

    private static readonly Move[] AllMoves =
    [
        Move.U, Move.Ui, Move.U2,
        Move.R, Move.Ri, Move.R2,
        Move.F, Move.Fi, Move.F2,
        Move.D, Move.Di, Move.D2,
        Move.L, Move.Li, Move.L2,
        Move.B, Move.Bi, Move.B2
    ];

    public SolverId Id => SolverId.TwoPhase;

    public bool CanSolve(SolveRequest request) =>
        request.InputType == SolveInputType.FaceletState &&
        request.SolverId is SolverId.Auto or SolverId.TwoPhase;

    public SolveResult Solve(SolveRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!CanSolve(request))
        {
            return Failure(SolveStatus.SolverUnavailable, "State solver only supports facelet-state input.", stopwatch.Elapsed);
        }

        var parseResult = CubeParser.ParseFaceletState(request.Input);
        if (!parseResult.IsSuccess)
        {
            return Failure(SolveStatus.InvalidState, string.Join(Environment.NewLine, parseResult.Errors.Select(error => error.Message)), stopwatch.Elapsed);
        }

        var physicalValidation = CubeValidator.ValidatePhysicalState(request.Input);
        if (!physicalValidation.IsValid)
        {
            return new SolveResult(
                SolveStatus.NotSolvable,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                parseResult.Value,
                parseResult.Value,
                string.Join(Environment.NewLine, physicalValidation.Errors.Select(error => error.Message)),
                stopwatch.Elapsed);
        }

        var initialState = parseResult.Value!;
        if (initialState.IsSolved())
        {
            return new SolveResult(
                SolveStatus.Solved,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                initialState,
                initialState,
                null,
                stopwatch.Elapsed);
        }

        var maxDepth = request.MaxDepth ?? DefaultMaxDepth;
        if (maxDepth < 0)
        {
            return Failure(SolveStatus.InvalidInput, "Max depth cannot be negative.", stopwatch.Elapsed);
        }

        var searchResult = Search(initialState, maxDepth, cancellationToken);
        stopwatch.Stop();

        if (searchResult.Moves is null)
        {
            return new SolveResult(
                SolveStatus.MaxDepthExceeded,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                initialState,
                initialState,
                $"No verified solution found within depth {maxDepth}.",
                stopwatch.Elapsed,
                searchResult.NodesExpanded);
        }

        var moves = new MoveSequence(searchResult.Moves).Normalize().Moves;
        var steps = request.IncludeSnapshots ? CreateSteps(initialState, moves, cancellationToken) : Array.Empty<SolveStep>();
        var verified = VerificationSolver.Verify(initialState, moves, out var finalState);

        return verified
            ? new SolveResult(SolveStatus.Solved, moves, steps, initialState, finalState, null, stopwatch.Elapsed, searchResult.NodesExpanded)
            : new SolveResult(
                SolveStatus.InternalError,
                Array.Empty<Move>(),
                Array.Empty<SolveStep>(),
                initialState,
                finalState,
                "State solver verification failed.",
                stopwatch.Elapsed,
                searchResult.NodesExpanded);
    }

    private static SearchResult Search(Cube initialState, int maxDepth, CancellationToken cancellationToken)
    {
        var fromInitial = new Dictionary<string, SearchNode>(StringComparer.Ordinal)
        {
            [initialState.ToFaceletString()] = new(Array.Empty<Move>(), null)
        };
        var fromSolved = new Dictionary<string, SearchNode>(StringComparer.Ordinal)
        {
            [Cube.SolvedState] = new(Array.Empty<Move>(), null)
        };
        var initialFrontier = new Dictionary<string, SearchNode>(fromInitial, StringComparer.Ordinal);
        var solvedFrontier = new Dictionary<string, SearchNode>(fromSolved, StringComparer.Ordinal);
        long nodesExpanded = 0;

        for (var depth = 0; depth < maxDepth; depth++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var expandInitial = initialFrontier.Count <= solvedFrontier.Count;
            var frontier = expandInitial ? initialFrontier : solvedFrontier;
            var visited = expandInitial ? fromInitial : fromSolved;
            var opposite = expandInitial ? fromSolved : fromInitial;
            var nextFrontier = new Dictionary<string, SearchNode>(StringComparer.Ordinal);

            foreach (var (state, node) in frontier)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var cube = new Cube(state);

                foreach (var move in CandidateMoves(node.LastFace))
                {
                    var nextCube = cube.Apply(move);
                    var nextState = nextCube.ToFaceletString();
                    if (visited.ContainsKey(nextState))
                    {
                        continue;
                    }

                    var nextPath = node.Path.Append(move).ToArray();
                    var nextNode = new SearchNode(nextPath, move.Face);
                    visited[nextState] = nextNode;
                    nextFrontier[nextState] = nextNode;
                    nodesExpanded++;

                    if (opposite.TryGetValue(nextState, out var oppositeNode))
                    {
                        var solution = expandInitial
                            ? JoinPaths(nextNode.Path, oppositeNode.Path)
                            : JoinPaths(oppositeNode.Path, nextNode.Path);
                        return new SearchResult(solution, nodesExpanded);
                    }
                }
            }

            if (expandInitial)
            {
                initialFrontier = nextFrontier;
            }
            else
            {
                solvedFrontier = nextFrontier;
            }
        }

        return new SearchResult(null, nodesExpanded);
    }

    private static IEnumerable<Move> CandidateMoves(Face? previousFace)
    {
        foreach (var move in AllMoves)
        {
            if (move.Face != previousFace)
            {
                yield return move;
            }
        }
    }

    private static Move[] JoinPaths(IReadOnlyList<Move> pathFromInitial, IReadOnlyList<Move> pathFromSolved) =>
        pathFromInitial.Concat(pathFromSolved.Reverse().Select(move => move.Inverse())).ToArray();

    private static IReadOnlyList<SolveStep> CreateSteps(Cube initialState, IReadOnlyList<Move> moves, CancellationToken cancellationToken)
    {
        var steps = new List<SolveStep>(moves.Count);
        var current = initialState;

        for (var i = 0; i < moves.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var before = current;
            current = current.Apply(moves[i]);
            steps.Add(new SolveStep(i + 1, moves[i], $"Apply {moves[i]}", SolverPhase.TwoPhasePhase1, before, current));
        }

        return steps;
    }

    private static SolveResult Failure(SolveStatus status, string message, TimeSpan elapsed) =>
        new(status, Array.Empty<Move>(), Array.Empty<SolveStep>(), null, null, message, elapsed);

    private sealed record SearchNode(IReadOnlyList<Move> Path, Face? LastFace);

    private sealed record SearchResult(IReadOnlyList<Move>? Moves, long NodesExpanded);
}
