using RubiksCube.Core.Validation;

namespace RubiksCube.Core;

public static class CubieCubeParser
{
    private static readonly CubieDefinition<CornerPosition>[] Corners =
    [
        new(CornerPosition.URF, [8, 9, 20], ['U', 'R', 'F']),
        new(CornerPosition.UFL, [6, 18, 38], ['U', 'F', 'L']),
        new(CornerPosition.ULB, [0, 36, 47], ['U', 'L', 'B']),
        new(CornerPosition.UBR, [2, 45, 11], ['U', 'B', 'R']),
        new(CornerPosition.DFR, [29, 26, 15], ['D', 'F', 'R']),
        new(CornerPosition.DLF, [27, 44, 24], ['D', 'L', 'F']),
        new(CornerPosition.DBL, [33, 53, 42], ['D', 'B', 'L']),
        new(CornerPosition.DRB, [35, 17, 51], ['D', 'R', 'B'])
    ];

    private static readonly CubieDefinition<EdgePosition>[] Edges =
    [
        new(EdgePosition.UR, [5, 10], ['U', 'R']),
        new(EdgePosition.UF, [7, 19], ['U', 'F']),
        new(EdgePosition.UL, [3, 37], ['U', 'L']),
        new(EdgePosition.UB, [1, 46], ['U', 'B']),
        new(EdgePosition.DR, [32, 16], ['D', 'R']),
        new(EdgePosition.DF, [28, 25], ['D', 'F']),
        new(EdgePosition.DL, [30, 43], ['D', 'L']),
        new(EdgePosition.DB, [34, 52], ['D', 'B']),
        new(EdgePosition.FR, [23, 12], ['F', 'R']),
        new(EdgePosition.FL, [21, 41], ['F', 'L']),
        new(EdgePosition.BL, [50, 39], ['B', 'L']),
        new(EdgePosition.BR, [48, 14], ['B', 'R'])
    ];

    public static ParseResult<CubieCube> Parse(Cube cube)
    {
        ArgumentNullException.ThrowIfNull(cube);

        var errors = new List<ParseError>();
        var cornerPermutation = new CornerPosition[8];
        var cornerOrientation = new byte[8];
        var edgePermutation = new EdgePosition[12];
        var edgeOrientation = new byte[12];
        var seenCorners = new HashSet<CornerPosition>();
        var seenEdges = new HashSet<EdgePosition>();

        for (var i = 0; i < Corners.Length; i++)
        {
            var stickers = Corners[i].Indices.Select(index => cube[index]).ToArray();
            if (!TryFindCorner(stickers, out var corner))
            {
                errors.Add(new ParseError(
                    "InvalidCornerCubie",
                    $"Corner at position {Corners[i].Position} has invalid stickers '{new string(stickers)}'.",
                    Index: i));
                continue;
            }

            if (!seenCorners.Add(corner.Position))
            {
                errors.Add(new ParseError("DuplicateCornerCubie", $"Corner cubie {corner.Position} appears more than once.", Index: i));
            }

            cornerPermutation[i] = corner.Position;
            cornerOrientation[i] = CornerOrientation(stickers);
        }

        for (var i = 0; i < Edges.Length; i++)
        {
            var stickers = Edges[i].Indices.Select(index => cube[index]).ToArray();
            if (!TryFindEdge(stickers, out var edge))
            {
                errors.Add(new ParseError(
                    "InvalidEdgeCubie",
                    $"Edge at position {Edges[i].Position} has invalid stickers '{new string(stickers)}'.",
                    Index: i));
                continue;
            }

            if (!seenEdges.Add(edge.Position))
            {
                errors.Add(new ParseError("DuplicateEdgeCubie", $"Edge cubie {edge.Position} appears more than once.", Index: i));
            }

            edgePermutation[i] = edge.Position;
            edgeOrientation[i] = EdgeOrientation(stickers, Edges[i]);
        }

        if (errors.Count != 0)
        {
            return ParseResult<CubieCube>.Failure(errors);
        }

        var cornerOrientationSum = cornerOrientation.Sum(value => value);
        if (cornerOrientationSum % 3 != 0)
        {
            errors.Add(new ParseError("InvalidCornerOrientation", "Corner orientation sum must be divisible by 3."));
        }

        var edgeOrientationSum = edgeOrientation.Sum(value => value);
        if (edgeOrientationSum % 2 != 0)
        {
            errors.Add(new ParseError("InvalidEdgeOrientation", "Edge orientation sum must be even."));
        }

        var cornerParity = PermutationParity(cornerPermutation.Select(value => (int)value).ToArray());
        var edgeParity = PermutationParity(edgePermutation.Select(value => (int)value).ToArray());
        if (cornerParity != edgeParity)
        {
            errors.Add(new ParseError("InvalidPermutationParity", "Corner and edge permutation parity must match."));
        }

        return errors.Count == 0
            ? ParseResult<CubieCube>.Success(new CubieCube(cornerPermutation, cornerOrientation, edgePermutation, edgeOrientation))
            : ParseResult<CubieCube>.Failure(errors);
    }

    public static CubeValidationResult ValidatePhysicalState(Cube cube)
    {
        var parseResult = Parse(cube);
        if (parseResult.IsSuccess)
        {
            return CubeValidationResult.Valid;
        }

        return new CubeValidationResult(parseResult.Errors.Select(error =>
            new CubeValidationError(error.Code, error.Message, error.Index)).ToArray());
    }

    private static bool TryFindCorner(char[] stickers, out CubieDefinition<CornerPosition> corner)
    {
        corner = default;
        var sorted = Sort(stickers);
        foreach (var candidate in Corners)
        {
            if (Sort(candidate.Colors).SequenceEqual(sorted))
            {
                corner = candidate;
                return true;
            }
        }

        return false;
    }

    private static bool TryFindEdge(char[] stickers, out CubieDefinition<EdgePosition> edge)
    {
        edge = default;
        var sorted = Sort(stickers);
        foreach (var candidate in Edges)
        {
            if (Sort(candidate.Colors).SequenceEqual(sorted))
            {
                edge = candidate;
                return true;
            }
        }

        return false;
    }

    private static byte CornerOrientation(char[] stickers)
    {
        for (var i = 0; i < stickers.Length; i++)
        {
            if (stickers[i] is 'U' or 'D')
            {
                return (byte)i;
            }
        }

        return 0;
    }

    private static byte EdgeOrientation(char[] stickers, CubieDefinition<EdgePosition> position)
    {
        var firstFace = FaceFromIndex(position.Indices[0]);
        var secondFace = FaceFromIndex(position.Indices[1]);

        for (var i = 0; i < stickers.Length; i++)
        {
            if (stickers[i] is 'U' or 'D')
            {
                var face = i == 0 ? firstFace : secondFace;
                return (byte)(face is Face.U or Face.D ? 0 : 1);
            }
        }

        for (var i = 0; i < stickers.Length; i++)
        {
            if (stickers[i] is 'F' or 'B')
            {
                var face = i == 0 ? firstFace : secondFace;
                return (byte)(face is Face.F or Face.B ? 0 : 1);
            }
        }

        return 0;
    }

    private static Face FaceFromIndex(int index) => (Face)(index / 9);

    private static int PermutationParity(int[] permutation)
    {
        var inversions = 0;
        for (var i = 0; i < permutation.Length; i++)
        {
            for (var j = i + 1; j < permutation.Length; j++)
            {
                if (permutation[i] > permutation[j])
                {
                    inversions++;
                }
            }
        }

        return inversions % 2;
    }

    private static char[] Sort(IEnumerable<char> values) => values.Order().ToArray();

    private readonly record struct CubieDefinition<T>(T Position, int[] Indices, char[] Colors);
}
