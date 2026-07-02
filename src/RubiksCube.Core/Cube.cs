namespace RubiksCube.Core;

public sealed class Cube : IEquatable<Cube>
{
    public const int StickerCount = 54;
    public const string SolvedState = "UUUUUUUUURRRRRRRRRFFFFFFFFFDDDDDDDDDLLLLLLLLLBBBBBBBBB";

    public static Cube Solved { get; } = new(SolvedState);

    private readonly string _facelets;

    public Cube(string facelets)
    {
        ArgumentNullException.ThrowIfNull(facelets);

        if (facelets.Length != StickerCount)
        {
            throw new ArgumentException($"Cube state must contain exactly {StickerCount} facelets.", nameof(facelets));
        }

        _facelets = facelets;
    }

    public char this[int index] => _facelets[index];

    public Cube Apply(Move move) => MoveTables.Apply(this, move);

    public Cube Apply(IEnumerable<Move> moves)
    {
        ArgumentNullException.ThrowIfNull(moves);
        return moves.Aggregate(this, (cube, move) => cube.Apply(move));
    }

    public bool IsSolved() => _facelets == SolvedState;

    public string ToFaceletString() => _facelets;

    public CubeNet ToNet() => new(
        Slice(Face.U),
        Slice(Face.R),
        Slice(Face.F),
        Slice(Face.D),
        Slice(Face.L),
        Slice(Face.B));

    public bool Equals(Cube? other) => other is not null && _facelets == other._facelets;

    public override bool Equals(object? obj) => Equals(obj as Cube);

    public override int GetHashCode() => _facelets.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => _facelets;

    private IReadOnlyList<char> Slice(Face face) => _facelets.Substring((int)face * 9, 9).ToCharArray();
}
