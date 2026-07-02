using RubiksCube.Core.Validation;

namespace RubiksCube.Core;

public static class CubeValidator
{
    private static readonly char[] ValidFacelets = ['U', 'R', 'F', 'D', 'L', 'B'];

    public static CubeValidationResult ValidateStickerState(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var normalized = RemoveWhitespace(input);
        var errors = new List<CubeValidationError>();

        if (normalized.Length != Cube.StickerCount)
        {
            errors.Add(new CubeValidationError(
                "InvalidLength",
                $"Cube state must contain exactly {Cube.StickerCount} facelets after whitespace is removed."));
        }

        for (var i = 0; i < normalized.Length; i++)
        {
            if (!ValidFacelets.Contains(normalized[i]))
            {
                errors.Add(new CubeValidationError(
                    "InvalidFacelet",
                    $"Invalid facelet '{normalized[i]}' at index {i}.",
                    i,
                    normalized[i]));
            }
        }

        foreach (var facelet in ValidFacelets)
        {
            var count = normalized.Count(value => value == facelet);
            if (count != 9)
            {
                errors.Add(new CubeValidationError(
                    "InvalidFaceletCount",
                    $"Facelet '{facelet}' must appear exactly 9 times, but appears {count} times.",
                    Value: facelet));
            }
        }

        if (normalized.Length == Cube.StickerCount)
        {
            foreach (var face in Enum.GetValues<Face>())
            {
                var centerIndex = FaceletIndex.Center(face);
                var expected = face.ToString()[0];
                var actual = normalized[centerIndex];
                if (actual != expected)
                {
                    errors.Add(new CubeValidationError(
                        "InvalidCenter",
                        $"Center at index {centerIndex} must be '{expected}', but was '{actual}'.",
                        centerIndex,
                        actual));
                }
            }
        }

        return errors.Count == 0 ? CubeValidationResult.Valid : new CubeValidationResult(errors);
    }

    public static CubeValidationResult ValidatePhysicalState(string input)
    {
        var stickerValidation = ValidateStickerState(input);
        if (!stickerValidation.IsValid)
        {
            return stickerValidation;
        }

        var cube = new Cube(RemoveWhitespace(input));
        return CubieCubeParser.ValidatePhysicalState(cube);
    }

    internal static string RemoveWhitespace(string input) => new(input.Where(value => !char.IsWhiteSpace(value)).ToArray());
}
