namespace RubiksCube.Core;

public static class ScrambleGenerator
{
    private static readonly Face[] Faces = Enum.GetValues<Face>();
    private static readonly TurnAmount[] Amounts =
    [
        TurnAmount.Clockwise,
        TurnAmount.Half,
        TurnAmount.CounterClockwise
    ];

    public static MoveSequence Generate(int length, int? seed = null)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "Scramble length cannot be negative.");
        }

        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var moves = new List<Move>(length);
        Face? previousFace = null;

        for (var i = 0; i < length; i++)
        {
            var face = Faces[random.Next(Faces.Length)];
            while (face == previousFace)
            {
                face = Faces[random.Next(Faces.Length)];
            }

            moves.Add(new Move(face, Amounts[random.Next(Amounts.Length)]));
            previousFace = face;
        }

        return new MoveSequence(moves);
    }
}
