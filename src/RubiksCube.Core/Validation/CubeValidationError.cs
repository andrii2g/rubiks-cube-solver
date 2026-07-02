namespace RubiksCube.Core.Validation;

public sealed record CubeValidationError(
    string Code,
    string Message,
    int? Index = null,
    char? Value = null);
