namespace RubiksCube.Core;

public static class MoveTables
{
    private static readonly IReadOnlyDictionary<Face, int[]> ClockwisePermutations = CreateClockwisePermutations();

    public static ReadOnlySpan<int> GetClockwisePermutation(Face face) => ClockwisePermutations[face];

    public static Cube Apply(Cube cube, Move move)
    {
        ArgumentNullException.ThrowIfNull(cube);

        var current = cube.ToFaceletString().ToCharArray();
        var turns = (int)move.Amount;
        var permutation = ClockwisePermutations[move.Face];

        for (var turn = 0; turn < turns; turn++)
        {
            var next = new char[Cube.StickerCount];

            // Permutation convention: newSticker[i] = oldSticker[permutation[i]].
            for (var i = 0; i < permutation.Length; i++)
            {
                next[i] = current[permutation[i]];
            }

            current = next;
        }

        return new Cube(new string(current));
    }

    private static IReadOnlyDictionary<Face, int[]> CreateClockwisePermutations()
    {
        var facelets = Enumerable.Range(0, Cube.StickerCount)
            .Select(index => new IndexedFacelet(index, ToFaceletCoordinate(index)))
            .ToArray();
        var indexByCoordinate = facelets.ToDictionary(item => item.Coordinate, item => item.Index);

        return Enum.GetValues<Face>().ToDictionary(face => face, face =>
        {
            var permutation = new int[Cube.StickerCount];
            var axis = AxisFor(face);
            var layer = LayerFor(face);
            var quarterTurns = ClockwiseQuarterTurnsFor(face);

            foreach (var item in facelets)
            {
                var coordinate = item.Coordinate;
                if (LayerValue(coordinate.Position, axis) == layer)
                {
                    coordinate = Rotate(coordinate, axis, quarterTurns);
                }

                var newIndex = indexByCoordinate[coordinate];
                permutation[newIndex] = item.Index;
            }

            return permutation;
        });
    }

    private static FaceletCoordinate ToFaceletCoordinate(int index)
    {
        var face = (Face)(index / 9);
        var local = index % 9;
        var row = local / 3;
        var column = local % 3;

        return face switch
        {
            Face.U => new FaceletCoordinate(new Vector3(column - 1, 1, row - 1), new Vector3(0, 1, 0)),
            Face.R => new FaceletCoordinate(new Vector3(1, 1 - row, 1 - column), new Vector3(1, 0, 0)),
            Face.F => new FaceletCoordinate(new Vector3(column - 1, 1 - row, 1), new Vector3(0, 0, 1)),
            Face.D => new FaceletCoordinate(new Vector3(column - 1, -1, 1 - row), new Vector3(0, -1, 0)),
            Face.L => new FaceletCoordinate(new Vector3(-1, 1 - row, column - 1), new Vector3(-1, 0, 0)),
            Face.B => new FaceletCoordinate(new Vector3(1 - column, 1 - row, -1), new Vector3(0, 0, -1)),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Invalid facelet index.")
        };
    }

    private static Axis AxisFor(Face face) => face switch
    {
        Face.U or Face.D => Axis.Y,
        Face.R or Face.L => Axis.X,
        Face.F or Face.B => Axis.Z,
        _ => throw new ArgumentOutOfRangeException(nameof(face), face, "Invalid face.")
    };

    private static int LayerFor(Face face) => face is Face.U or Face.R or Face.F ? 1 : -1;

    private static int ClockwiseQuarterTurnsFor(Face face) => LayerFor(face) == 1 ? -1 : 1;

    private static int LayerValue(Vector3 vector, Axis axis) => axis switch
    {
        Axis.X => vector.X,
        Axis.Y => vector.Y,
        Axis.Z => vector.Z,
        _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid axis.")
    };

    private static FaceletCoordinate Rotate(FaceletCoordinate coordinate, Axis axis, int quarterTurns) =>
        new(Rotate(coordinate.Position, axis, quarterTurns), Rotate(coordinate.Normal, axis, quarterTurns));

    private static Vector3 Rotate(Vector3 vector, Axis axis, int quarterTurns) => axis switch
    {
        Axis.X => quarterTurns == 1
            ? new Vector3(vector.X, -vector.Z, vector.Y)
            : new Vector3(vector.X, vector.Z, -vector.Y),
        Axis.Y => quarterTurns == 1
            ? new Vector3(vector.Z, vector.Y, -vector.X)
            : new Vector3(-vector.Z, vector.Y, vector.X),
        Axis.Z => quarterTurns == 1
            ? new Vector3(-vector.Y, vector.X, vector.Z)
            : new Vector3(vector.Y, -vector.X, vector.Z),
        _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid axis.")
    };

    private enum Axis
    {
        X,
        Y,
        Z
    }

    private readonly record struct Vector3(int X, int Y, int Z);

    private readonly record struct FaceletCoordinate(Vector3 Position, Vector3 Normal);

    private readonly record struct IndexedFacelet(int Index, FaceletCoordinate Coordinate);
}
