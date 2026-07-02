namespace RubiksCube.Core;

public sealed record CubeNet(
    IReadOnlyList<char> U,
    IReadOnlyList<char> R,
    IReadOnlyList<char> F,
    IReadOnlyList<char> D,
    IReadOnlyList<char> L,
    IReadOnlyList<char> B);
