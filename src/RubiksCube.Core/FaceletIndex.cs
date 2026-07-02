namespace RubiksCube.Core;

public static class FaceletIndex
{
    public static int Start(Face face) => (int)face * 9;

    public static int Center(Face face) => Start(face) + 4;
}
