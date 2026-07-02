namespace RubiksCube.Core;

public sealed record ParseResult<T>(
    bool IsSuccess,
    T? Value,
    IReadOnlyList<ParseError> Errors)
{
    public static ParseResult<T> Success(T value) => new(true, value, Array.Empty<ParseError>());

    public static ParseResult<T> Failure(IEnumerable<ParseError> errors) =>
        new(false, default, errors.ToArray());
}

public sealed record ParseError(
    string Code,
    string Message,
    string? Token = null,
    int? Position = null,
    int? Index = null);
