using RubiksCube.Core;

namespace RubiksCube.Solver;

public sealed record SolveResult(
    SolveStatus Status,
    IReadOnlyList<Move> Moves,
    IReadOnlyList<SolveStep> Steps,
    Cube? InitialState,
    Cube? FinalState,
    string? Message,
    TimeSpan Elapsed,
    long? NodesExpanded = null)
{
    public bool Verified => Status == SolveStatus.Solved && FinalState?.IsSolved() == true;
}
