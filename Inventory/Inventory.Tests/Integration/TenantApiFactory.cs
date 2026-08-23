using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Inventory.Tests.Integration;

/// <summary>
/// Test host wired with three registered tenants (default, acme, globex) so
/// Checkpoint B integration tests can exercise resolution, CORS-adjacent
/// dot-anchored matching, and JWT tenant-claim cross-validation.
///
/// Connection strings for "acme" and "globex" come from
/// INVENTORY_TEST_ACME_CONNECTION / INVENTORY_TEST_GLOBEX_CONNECTION when a
/// real per-tenant SQL Server database is available (see proposal.md
/// Dependencies: "the user will create the test tenant DB"). When unset, a
/// syntactically valid but undialed placeholder is used — enough to satisfy
/// TenantRegistryOptionsValidator's ValidateOnStart, and sufficient for every
/// scenario that short-circuits before the DbContext is ever touched
/// (unknown/unsupported host, cross-tenant claim mismatch).
///
/// GLOBEX is NOT optional for <see cref="HasRealDatabases"/> — it must be set
/// alongside DEFAULT and ACME, because
/// SameEmailInTwoTenants_AuthenticatesIndependently exercises acme AND globex
/// together (Judgment Day implementation round 1, Judge B).
/// </summary>
public class TenantApiFactory : WebApplicationFactory<Program>
{
    public const string PlaceholderConnectionString =
        "Data Source=(local);Initial Catalog=__not_dialed__;Integrated Security=false;User ID=none;Password=none;Connect Timeout=1";

    // Fixed by Judgment Day implementation round 1 (Judge B, WARNING): this
    // previously checked only DEFAULT and ACME, but
    // SameEmailInTwoTenants_AuthenticatesIndependently logs into acme AND
    // globex — with GLOBEX unset it ran anyway against the undialed
    // PlaceholderConnectionString instead of skipping with a clear message.
    public static bool HasRealDatabases =>
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION"))
        && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INVENTORY_TEST_ACME_CONNECTION"))
        && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INVENTORY_TEST_GLOBEX_CONNECTION"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tenants:RootDomains:0"] = "localhost",
                ["Tenants:RootDomains:1"] = "127.0.0.1",
                ["Tenants:RootDomains:2"] = "tuapp.com",
                ["Tenants:DefaultKey"] = "default",
                ["Tenants:Registry:default:Name"] = "Mi Empresa",
                ["Tenants:Registry:default:ConnectionString"] =
                    Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")
                        ?? PlaceholderConnectionString,
                ["Tenants:Registry:acme:Name"] = "Acme S.A.",
                ["Tenants:Registry:acme:ConnectionString"] =
                    Environment.GetEnvironmentVariable("INVENTORY_TEST_ACME_CONNECTION")
                        ?? PlaceholderConnectionString,
                ["Tenants:Registry:globex:Name"] = "Globex Corp.",
                ["Tenants:Registry:globex:ConnectionString"] =
                    Environment.GetEnvironmentVariable("INVENTORY_TEST_GLOBEX_CONNECTION")
                        ?? PlaceholderConnectionString,
            });
        });
    }
}
