using Inventory.Api.Tenancy;
using Xunit;

namespace Inventory.Tests.Tenancy;

public class TenantResolverTests
{
    private static TenantRegistryOptions BuildOptions() => new()
    {
        RootDomains = new List<string> { "tuapp.com", "localhost", "127.0.0.1" },
        DefaultKey = "default",
        Registry = new Dictionary<string, TenantEntryOptions>
        {
            ["default"] = new TenantEntryOptions { Name = "Mi Empresa", ConnectionString = "Server=default;" },
            ["acme"] = new TenantEntryOptions { Name = "Acme S.A.", ConnectionString = "Server=acme;" },
        },
    };

    [Fact]
    public void ResolveKey_ApexRootDomain_ResolvesToDefault()
    {
        var result = TenantResolver.ResolveKey("tuapp.com", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("default", resolved.Key);
    }

    [Fact]
    public void ResolveKey_WwwSubdomain_ResolvesToDefault()
    {
        var result = TenantResolver.ResolveKey("www.tuapp.com", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("default", resolved.Key);
    }

    [Fact]
    public void ResolveKey_AcmeLocalhostSubdomain_ResolvesToAcme()
    {
        var result = TenantResolver.ResolveKey("acme.localhost", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("acme", resolved.Key);
    }

    [Fact]
    public void ResolveKey_AcmeTuappSubdomain_ResolvesToAcme()
    {
        var result = TenantResolver.ResolveKey("acme.tuapp.com", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("acme", resolved.Key);
    }

    [Fact]
    public void ResolveKey_LoopbackIp_ResolvesToDefault()
    {
        var result = TenantResolver.ResolveKey("127.0.0.1", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("default", resolved.Key);
    }

    [Fact]
    public void ResolveKey_NestedLabelNotInRegistry_ReturnsUnknownTenant()
    {
        var result = TenantResolver.ResolveKey("a.b.tuapp.com", BuildOptions());

        var unknown = Assert.IsType<TenantResolution.UnknownTenant>(result);
        Assert.Equal("a.b", unknown.Key);
    }

    [Fact]
    public void ResolveKey_HostMatchingNoRootDomain_ReturnsUnsupportedHost()
    {
        var result = TenantResolver.ResolveKey("example.org", BuildOptions());

        Assert.IsType<TenantResolution.UnsupportedHost>(result);
    }

    [Fact]
    public void ResolveKey_UpperCaseHost_ResolvesCaseInsensitively()
    {
        var result = TenantResolver.ResolveKey("ACME.TUAPP.COM", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("acme", resolved.Key);
    }

    [Fact]
    public void ResolveKey_PortBearingHost_StripsPortBeforeResolving()
    {
        var result = TenantResolver.ResolveKey("acme.localhost:4200", BuildOptions());

        var resolved = Assert.IsType<TenantResolution.Resolved>(result);
        Assert.Equal("acme", resolved.Key);
    }

    [Theory]
    [InlineData("eviltuapp.com")]
    [InlineData("nottuapp.com")]
    public void ResolveKey_DotBoundaryBypassAttempt_DoesNotMatchRoot(string host)
    {
        var result = TenantResolver.ResolveKey(host, BuildOptions());

        Assert.IsType<TenantResolution.UnsupportedHost>(result);
    }
}
