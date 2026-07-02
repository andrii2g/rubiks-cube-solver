using RubiksCube.Core;

namespace RubiksCube.Core.Tests;

public sealed class CubeNetTests
{
    [Fact]
    public void ToNet_MapsSolvedFacesInCanonicalOrder()
    {
        var net = Cube.Solved.ToNet();

        Assert.Equal("UUUUUUUUU", new string(net.U.ToArray()));
        Assert.Equal("RRRRRRRRR", new string(net.R.ToArray()));
        Assert.Equal("FFFFFFFFF", new string(net.F.ToArray()));
        Assert.Equal("DDDDDDDDD", new string(net.D.ToArray()));
        Assert.Equal("LLLLLLLLL", new string(net.L.ToArray()));
        Assert.Equal("BBBBBBBBB", new string(net.B.ToArray()));
    }
}
