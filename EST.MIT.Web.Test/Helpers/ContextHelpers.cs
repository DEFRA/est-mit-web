using Microsoft.AspNetCore.Http;

namespace Helpers.Tests;
public class ContextHelpersTests
{
    [Fact]
    public void GetBaseURI_Returns_String()
    {

        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost");

        context.GetBaseURI().Should().Be("http://localhost");
    }
}