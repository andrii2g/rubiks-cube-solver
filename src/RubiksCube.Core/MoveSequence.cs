using System.Collections.ObjectModel;

namespace RubiksCube.Core;

public sealed class MoveSequence
{
    public MoveSequence(IEnumerable<Move> moves)
    {
        Moves = new ReadOnlyCollection<Move>(moves.ToArray());
    }

    public IReadOnlyList<Move> Moves { get; }

    public MoveSequence Inverse() => new(Moves.Reverse().Select(move => move.Inverse()));

    public MoveSequence Normalize()
    {
        var normalized = new List<Move>();

        foreach (var move in Moves)
        {
            if (normalized.Count == 0 || normalized[^1].Face != move.Face)
            {
                normalized.Add(move);
                continue;
            }

            var previous = normalized[^1];
            var amount = ((int)previous.Amount + (int)move.Amount) % 4;
            normalized.RemoveAt(normalized.Count - 1);

            if (amount != 0)
            {
                normalized.Add(new Move(move.Face, (TurnAmount)amount));
            }
        }

        return new MoveSequence(normalized);
    }

    public override string ToString() => string.Join(' ', Moves.Select(move => move.ToString()));
}
