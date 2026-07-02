using RubiksCube.Core;

namespace RubiksCube.Solver;

public static class VerificationSolver
{
    public static bool Verify(Cube initialState, IEnumerable<Move> moves, out Cube finalState)
    {
        finalState = initialState.Apply(moves);
        return finalState.IsSolved();
    }
}
