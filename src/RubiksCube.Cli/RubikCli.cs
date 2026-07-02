using System.Text.Json;
using RubiksCube.Core;
using RubiksCube.Solver;

namespace RubiksCube.Cli;

public static class RubikCli
{
    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            WriteHelp(output);
            return 0;
        }

        return args[0] switch
        {
            "solve" => Solve(args[1..], output, error),
            "validate" => Validate(args[1..], output, error),
            "random" => Random(args[1..], output, error),
            "visualize" => Visualize(args[1..], output, error),
            "normalize" => Normalize(args[1..], output, error),
            _ => Fail(error, $"Unknown command '{args[0]}'.")
        };
    }

    private static int Solve(string[] args, TextWriter output, TextWriter error)
    {
        var options = ParseOptions(args);
        var hasScramble = options.TryGetValue("scramble", out var scramble);
        var hasState = options.TryGetValue("state", out var state);

        if (hasScramble == hasState)
        {
            return Fail(error, "Exactly one of --scramble or --state is required.");
        }

        var format = options.GetValueOrDefault("format", "text");
        var request = hasScramble
            ? new SolveRequest(SolveInputType.Scramble, scramble!)
            : new SolveRequest(SolveInputType.FaceletState, state!);

        var result = new CubeSolverService().Solve(request);
        if (format == "json")
        {
            WriteJsonSolveResult(output, request.InputType, result);
        }
        else
        {
            WriteTextSolveResult(output, request.InputType, request.Input, result, options.ContainsKey("show-steps"));
        }

        return result.Status == SolveStatus.Solved ? 0 : 2;
    }

    private static int Validate(string[] args, TextWriter output, TextWriter error)
    {
        var options = ParseOptions(args);
        if (!options.TryGetValue("state", out var state))
        {
            return Fail(error, "--state is required.");
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return Fail(error, "--state requires a value.");
        }

        var result = CubeParser.ParseFaceletState(state);
        output.WriteLine($"Valid: {(result.IsSuccess ? "yes" : "no")}");
        output.WriteLine($"Solvable: {(result.IsSuccess ? "not checked" : "no")}");

        if (result.IsSuccess)
        {
            return 0;
        }

        output.WriteLine("Errors:");
        foreach (var parseError in result.Errors)
        {
            output.WriteLine($"- {parseError.Message}");
        }

        return 2;
    }

    private static int Random(string[] args, TextWriter output, TextWriter error)
    {
        var options = ParseOptions(args);
        var length = ParseIntOption(options, "length", 25, error);
        if (length is null)
        {
            return 2;
        }

        var seed = ParseOptionalIntOption(options, "seed", error);
        if (seed.HasError)
        {
            return 2;
        }

        output.WriteLine(ScrambleGenerator.Generate(length.Value, seed.Value).ToString());
        return 0;
    }

    private static int Visualize(string[] args, TextWriter output, TextWriter error)
    {
        var options = ParseOptions(args);
        Cube cube;

        if (options.TryGetValue("scramble", out var scramble))
        {
            if (string.IsNullOrWhiteSpace(scramble))
            {
                return Fail(error, "--scramble requires a value.");
            }

            var parseResult = MoveParser.ParseSequence(scramble);
            if (!parseResult.IsSuccess)
            {
                return Fail(error, parseResult.Errors[0].Message);
            }

            cube = Cube.Solved.Apply(parseResult.Value!.Moves);
        }
        else if (options.TryGetValue("state", out var state))
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return Fail(error, "--state requires a value.");
            }

            var parseResult = CubeParser.ParseFaceletState(state);
            if (!parseResult.IsSuccess)
            {
                return Fail(error, parseResult.Errors[0].Message);
            }

            cube = parseResult.Value!;
        }
        else
        {
            cube = Cube.Solved;
        }

        output.Write(ConsoleCubeRenderer.Render(cube));
        return 0;
    }

    private static int Normalize(string[] args, TextWriter output, TextWriter error)
    {
        var options = ParseOptions(args);
        if (!options.TryGetValue("scramble", out var scramble))
        {
            scramble = string.Join(' ', args);
        }

        if (scramble is null)
        {
            return Fail(error, "--scramble requires a value.");
        }

        var parseResult = MoveParser.ParseSequence(scramble);
        if (!parseResult.IsSuccess)
        {
            return Fail(error, parseResult.Errors[0].Message);
        }

        output.WriteLine(parseResult.Value!.Normalize().ToString());
        return 0;
    }

    private static Dictionary<string, string?> ParseOptions(string[] args)
    {
        var options = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var token = args[i];
            if (!token.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var name = token[2..];
            if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
            {
                options[name] = args[++i];
            }
            else
            {
                options[name] = null;
            }
        }

        return options;
    }

    private static int? ParseIntOption(Dictionary<string, string?> options, string name, int defaultValue, TextWriter error)
    {
        if (!options.TryGetValue(name, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        error.WriteLine($"--{name} must be an integer.");
        return null;
    }

    private static OptionalInt ParseOptionalIntOption(Dictionary<string, string?> options, string name, TextWriter error)
    {
        if (!options.TryGetValue(name, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return new OptionalInt(null, false);
        }

        if (int.TryParse(value, out var parsed))
        {
            return new OptionalInt(parsed, false);
        }

        error.WriteLine($"--{name} must be an integer.");
        return new OptionalInt(null, true);
    }

    private static void WriteTextSolveResult(TextWriter output, SolveInputType inputType, string input, SolveResult result, bool showSteps)
    {
        output.WriteLine($"Input type: {inputType}");
        output.WriteLine($"Input:      {input}");
        output.WriteLine($"Status:     {result.Status}");
        output.WriteLine($"Solution:   {string.Join(' ', result.Moves.Select(move => move.ToString()))}");
        output.WriteLine($"Moves:      {result.Moves.Count}");
        output.WriteLine($"Verified:   {(result.Verified ? "yes" : "no")}");

        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            output.WriteLine($"Message:    {result.Message}");
        }

        if (showSteps && result.Steps.Count > 0)
        {
            output.WriteLine();
            output.WriteLine("Steps:");
            foreach (var step in result.Steps)
            {
                output.WriteLine($"  {step.Index}. {step.Move}");
            }
        }
    }

    private static void WriteJsonSolveResult(TextWriter output, SolveInputType inputType, SolveResult result)
    {
        var payload = new
        {
            status = result.Status.ToString(),
            inputType = inputType.ToString(),
            solution = result.Moves.Select(move => move.ToString()).ToArray(),
            moveCount = result.Moves.Count,
            verified = result.Verified,
            initialState = result.InitialState?.ToFaceletString(),
            finalState = result.FinalState?.ToFaceletString(),
            message = result.Message
        };

        output.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static void WriteHelp(TextWriter output)
    {
        output.WriteLine("rubik solve --scramble \"R U R' U'\" [--format json] [--show-steps]");
        output.WriteLine("rubik solve --state <54-facelets>");
        output.WriteLine("rubik validate --state <54-facelets>");
        output.WriteLine("rubik random --length 25 [--seed 123]");
        output.WriteLine("rubik visualize [--scramble <moves> | --state <54-facelets>]");
        output.WriteLine("rubik normalize --scramble <moves>");
    }

    private static int Fail(TextWriter error, string message)
    {
        error.WriteLine(message);
        return 2;
    }

    private readonly record struct OptionalInt(int? Value, bool HasError);
}
