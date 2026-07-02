using RubiksCube.Core;
using RubiksCube.Solver;
using RubiksCube.Web.Models;

namespace RubiksCube.Web.Services;

public sealed class SolverViewService
{
    private readonly CubeSolverService _solver = new();

    public void ApplyInput(SolverPageState state)
    {
        state.ErrorMessage = null;
        state.SolveResult = null;
        state.CurrentStepIndex = 0;

        if (state.InputMode == InputMode.Scramble)
        {
            var parseResult = MoveParser.ParseSequence(state.ScrambleText);
            if (!parseResult.IsSuccess)
            {
                state.ErrorMessage = parseResult.Errors[0].Message;
                return;
            }

            state.InitialCube = Cube.Solved.Apply(parseResult.Value!.Moves);
            return;
        }

        var cubeResult = CubeParser.ParseFaceletState(state.StateText);
        if (!cubeResult.IsSuccess)
        {
            state.ErrorMessage = cubeResult.Errors[0].Message;
            return;
        }

        state.InitialCube = cubeResult.Value!;
    }

    public void Solve(SolverPageState state)
    {
        state.ErrorMessage = null;
        var request = state.InputMode == InputMode.Scramble
            ? new SolveRequest(SolveInputType.Scramble, state.ScrambleText)
            : new SolveRequest(SolveInputType.FaceletState, state.StateText);

        var result = _solver.Solve(request);
        state.SolveResult = result;
        state.InitialCube = result.InitialState ?? state.InitialCube;
        state.CurrentStepIndex = 0;

        if (result.Status != SolveStatus.Solved)
        {
            state.ErrorMessage = result.Message ?? result.Status.ToString();
        }
    }

    public void SetRandomScramble(SolverPageState state, int length = 25)
    {
        state.InputMode = InputMode.Scramble;
        state.ScrambleText = ScrambleGenerator.Generate(length).ToString();
        ApplyInput(state);
    }
}
