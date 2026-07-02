namespace RubiksCube.Solver;

public enum SolveStatus
{
    Solved,
    InvalidInput,
    InvalidState,
    NotSolvable,
    MaxDepthExceeded,
    SolverUnavailable,
    InternalError
}
