using RubiksCube.Core;
using RubiksCube.Solver;

namespace RubiksCube.Web.Models;

public enum InputMode
{
    Scramble,
    State
}

public sealed class SolverPageState
{
    public InputMode InputMode { get; set; } = InputMode.Scramble;
    public string ScrambleText { get; set; } = "R U R' U'";
    public string StateText { get; set; } = Cube.SolvedState;
    public Cube InitialCube { get; set; } = Cube.Solved;
    public SolveResult? SolveResult { get; set; }
    public int CurrentStepIndex { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsBusy { get; set; }
    public string? ErrorMessage { get; set; }

    public int StepCount => SolveResult?.Moves.Count ?? 0;

    public Cube CurrentCube
    {
        get
        {
            if (SolveResult is null || CurrentStepIndex == 0)
            {
                return InitialCube;
            }

            return SolveResult.Steps[Math.Min(CurrentStepIndex, SolveResult.Steps.Count) - 1].StateAfter;
        }
    }

    public Cube? PreviousCube
    {
        get
        {
            if (SolveResult is null || CurrentStepIndex == 0)
            {
                return null;
            }

            return CurrentStepIndex == 1
                ? InitialCube
                : SolveResult.Steps[CurrentStepIndex - 2].StateAfter;
        }
    }
}
