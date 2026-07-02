namespace RubiksCube.Core;

public static class MoveParser
{
    public static ParseResult<MoveSequence> ParseSequence(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var moves = new List<Move>();
        var errors = new List<ParseError>();
        var index = 0;

        while (index < input.Length)
        {
            while (index < input.Length && char.IsWhiteSpace(input[index]))
            {
                index++;
            }

            if (index >= input.Length)
            {
                break;
            }

            var tokenStart = index;
            while (index < input.Length && !char.IsWhiteSpace(input[index]))
            {
                index++;
            }

            var token = input[tokenStart..index];
            if (TryParseMove(token, out var move))
            {
                moves.Add(move);
            }
            else
            {
                errors.Add(CreateUnsupportedTokenError(token, tokenStart, moves.Count + errors.Count));
            }
        }

        return errors.Count == 0
            ? ParseResult<MoveSequence>.Success(new MoveSequence(moves))
            : ParseResult<MoveSequence>.Failure(errors);
    }

    private static bool TryParseMove(string token, out Move move)
    {
        move = default;

        if (token.Length is < 1 or > 2 || !TryParseFace(token[0], out var face))
        {
            return false;
        }

        var amount = token.Length == 1
            ? TurnAmount.Clockwise
            : token[1] switch
            {
                '\'' => TurnAmount.CounterClockwise,
                'i' => TurnAmount.CounterClockwise,
                '2' => TurnAmount.Half,
                _ => 0
            };

        if (amount == 0)
        {
            return false;
        }

        move = new Move(face, amount);
        return true;
    }

    private static bool TryParseFace(char value, out Face face)
    {
        face = value switch
        {
            'U' => Face.U,
            'R' => Face.R,
            'F' => Face.F,
            'D' => Face.D,
            'L' => Face.L,
            'B' => Face.B,
            _ => default
        };

        return value is 'U' or 'R' or 'F' or 'D' or 'L' or 'B';
    }

    private static ParseError CreateUnsupportedTokenError(string token, int position, int index)
    {
        var message = token switch
        {
            "M" or "E" or "S" => $"Unsupported move token \"{token}\". Slice moves are not supported in MVP.",
            "x" or "y" or "z" => $"Unsupported move token \"{token}\". Cube rotations are not supported in MVP.",
            _ when token.EndsWith('w') => $"Unsupported move token \"{token}\". Wide moves are not supported in MVP.",
            _ when token.Length == 1 && char.IsLower(token[0]) =>
                $"Unsupported move token \"{token}\". Lowercase notation is not supported in MVP.",
            _ => $"Unsupported move token \"{token}\"."
        };

        return new ParseError("UnsupportedMoveToken", message, token, position, index);
    }
}
