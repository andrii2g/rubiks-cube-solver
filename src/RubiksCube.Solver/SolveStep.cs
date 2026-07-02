using RubiksCube.Core;

namespace RubiksCube.Solver;

public sealed record SolveStep(
    int Index,
    Move? Move,
    string Description,
    SolverPhase Phase,
    Cube StateBefore,
    Cube StateAfter);
