namespace RubiksCube.Core;

public static class CubeParser
{
    public static ParseResult<Cube> ParseFaceletState(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var normalized = CubeValidator.RemoveWhitespace(input);
        var validation = CubeValidator.ValidateStickerState(input);
        if (!validation.IsValid)
        {
            return ParseResult<Cube>.Failure(validation.Errors.Select(error =>
                new ParseError(error.Code, error.Message, Position: error.Index, Index: error.Index)));
        }

        return ParseResult<Cube>.Success(new Cube(normalized));
    }
}
