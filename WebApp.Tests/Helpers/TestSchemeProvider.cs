using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebApp.Tests.Helpers;

public sealed class TestSchemeProvider : AuthenticationSchemeProvider
{
    public TestSchemeProvider(IOptions<AuthenticationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        if (name == IdentityConstants.ApplicationScheme)
        {
            return base.GetSchemeAsync(TestAuthHandler.Scheme);
        }

        return base.GetSchemeAsync(name);
    }
}
