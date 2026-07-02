using System.Text;
using RubiksCube.Core;

namespace RubiksCube.Cli;

public static class ConsoleCubeRenderer
{
    public static string Render(Cube cube)
    {
        var net = cube.ToNet();
        var builder = new StringBuilder();

        AppendFaceRow(builder, net.U, 0, "      ");
        AppendFaceRow(builder, net.U, 3, "      ");
        AppendFaceRow(builder, net.U, 6, "      ");
        builder.AppendLine();

        for (var row = 0; row < 3; row++)
        {
            AppendInlineFaceRow(builder, net.L, row);
            builder.Append("  ");
            AppendInlineFaceRow(builder, net.F, row);
            builder.Append("  ");
            AppendInlineFaceRow(builder, net.R, row);
            builder.Append("  ");
            AppendInlineFaceRow(builder, net.B, row);
            builder.AppendLine();
        }

        builder.AppendLine();
        AppendFaceRow(builder, net.D, 0, "      ");
        AppendFaceRow(builder, net.D, 3, "      ");
        AppendFaceRow(builder, net.D, 6, "      ");

        return builder.ToString();
    }

    private static void AppendFaceRow(StringBuilder builder, IReadOnlyList<char> face, int start, string prefix)
    {
        builder.Append(prefix);
        builder.Append(face[start]);
        builder.Append(' ');
        builder.Append(face[start + 1]);
        builder.Append(' ');
        builder.Append(face[start + 2]);
        builder.AppendLine();
    }

    private static void AppendInlineFaceRow(StringBuilder builder, IReadOnlyList<char> face, int row)
    {
        var start = row * 3;
        builder.Append(face[start]);
        builder.Append(' ');
        builder.Append(face[start + 1]);
        builder.Append(' ');
        builder.Append(face[start + 2]);
    }
}
