namespace RubiksCube.Cli;

public static class Program
{
    public static int Main(string[] args) => RubikCli.Run(args, Console.Out, Console.Error);
}
