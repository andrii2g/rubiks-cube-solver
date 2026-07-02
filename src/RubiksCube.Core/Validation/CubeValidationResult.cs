namespace RubiksCube.Core.Validation;

public sealed record CubeValidationResult(IReadOnlyList<CubeValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;

    public static CubeValidationResult Valid { get; } = new(Array.Empty<CubeValidationError>());
}
