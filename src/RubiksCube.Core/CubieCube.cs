namespace RubiksCube.Core;

public sealed class CubieCube
{
    public CubieCube(
        IReadOnlyList<CornerPosition> cornerPermutation,
        IReadOnlyList<byte> cornerOrientation,
        IReadOnlyList<EdgePosition> edgePermutation,
        IReadOnlyList<byte> edgeOrientation)
    {
        if (cornerPermutation.Count != 8 || cornerOrientation.Count != 8)
        {
            throw new ArgumentException("Corner arrays must contain 8 entries.");
        }

        if (edgePermutation.Count != 12 || edgeOrientation.Count != 12)
        {
            throw new ArgumentException("Edge arrays must contain 12 entries.");
        }

        CornerPermutation = cornerPermutation.ToArray();
        CornerOrientation = cornerOrientation.ToArray();
        EdgePermutation = edgePermutation.ToArray();
        EdgeOrientation = edgeOrientation.ToArray();
    }

    public CornerPosition[] CornerPermutation { get; }

    public byte[] CornerOrientation { get; }

    public EdgePosition[] EdgePermutation { get; }

    public byte[] EdgeOrientation { get; }

    public static CubieCube Solved { get; } = new(
        Enum.GetValues<CornerPosition>(),
        new byte[8],
        Enum.GetValues<EdgePosition>(),
        new byte[12]);

    public bool IsSolved() =>
        CornerPermutation.SequenceEqual(Solved.CornerPermutation) &&
        CornerOrientation.All(value => value == 0) &&
        EdgePermutation.SequenceEqual(Solved.EdgePermutation) &&
        EdgeOrientation.All(value => value == 0);
}
