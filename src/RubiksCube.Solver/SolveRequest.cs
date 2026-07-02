namespace RubiksCube.Solver;

public sealed record SolveRequest(
    SolveInputType InputType,
    string Input,
    SolverId SolverId = SolverId.Auto,
    int? MaxDepth = null,
    bool IncludeSnapshots = true);
