namespace RubiksCube.Core;

public readonly record struct Move(Face Face, TurnAmount Amount)
{
    public static Move U => new(Face.U, TurnAmount.Clockwise);
    public static Move Ui => new(Face.U, TurnAmount.CounterClockwise);
    public static Move U2 => new(Face.U, TurnAmount.Half);
    public static Move R => new(Face.R, TurnAmount.Clockwise);
    public static Move Ri => new(Face.R, TurnAmount.CounterClockwise);
    public static Move R2 => new(Face.R, TurnAmount.Half);
    public static Move F => new(Face.F, TurnAmount.Clockwise);
    public static Move Fi => new(Face.F, TurnAmount.CounterClockwise);
    public static Move F2 => new(Face.F, TurnAmount.Half);
    public static Move D => new(Face.D, TurnAmount.Clockwise);
    public static Move Di => new(Face.D, TurnAmount.CounterClockwise);
    public static Move D2 => new(Face.D, TurnAmount.Half);
    public static Move L => new(Face.L, TurnAmount.Clockwise);
    public static Move Li => new(Face.L, TurnAmount.CounterClockwise);
    public static Move L2 => new(Face.L, TurnAmount.Half);
    public static Move B => new(Face.B, TurnAmount.Clockwise);
    public static Move Bi => new(Face.B, TurnAmount.CounterClockwise);
    public static Move B2 => new(Face.B, TurnAmount.Half);

    public Move Inverse() => Amount switch
    {
        TurnAmount.Clockwise => new Move(Face, TurnAmount.CounterClockwise),
        TurnAmount.CounterClockwise => new Move(Face, TurnAmount.Clockwise),
        TurnAmount.Half => this,
        _ => throw new InvalidOperationException($"Unsupported turn amount: {Amount}.")
    };

    public override string ToString() => Amount switch
    {
        TurnAmount.Clockwise => Face.ToString(),
        TurnAmount.CounterClockwise => $"{Face}'",
        TurnAmount.Half => $"{Face}2",
        _ => throw new InvalidOperationException($"Unsupported turn amount: {Amount}.")
    };
}
