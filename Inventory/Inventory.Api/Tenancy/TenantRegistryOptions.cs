using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Inventory.Api.Tenancy;

/// <summary>
/// Bound once from the "Tenants" configuration section at startup
/// (<see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/>). A restart is
/// required to add a tenant — see design.md "IOptions bound once" decision.
/// </summary>
public class TenantRegistryOptions
{
    public const string SectionName = "Tenants";

    public List<string> RootDomains { get; set; } = new();

    public string DefaultKey { get; set; } = string.Empty;

    public Dictionary<string, TenantEntryOptions> Registry { get; set; } = new();
}

public class TenantEntryOptions
{
    public string Name { get; set; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;
}

/// <summary>
/// Runs at <c>ValidateOnStart</c> time. Order matters and must stay inside this
/// single validator (design.md "config schema, secrets, and default-tenant
/// compatibility" — Judgment Day round 1, Judge B): shim the default tenant's
/// connection string from the legacy <c>ConnectionStrings:SqlServerConnection</c>
/// key FIRST (in memory), THEN validate. Validating before shimming would reject
/// the exact empty-default-connection-string state the shim exists to support.
/// </summary>
public class TenantRegistryOptionsValidator(IConfiguration configuration)
    : IValidateOptions<TenantRegistryOptions>
{
    public ValidateOptionsResult Validate(string? name, TenantRegistryOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.DefaultKey))
            return ValidateOptionsResult.Fail("Tenants:DefaultKey is required.");

        if (!options.Registry.ContainsKey(options.DefaultKey))
            return ValidateOptionsResult.Fail(
                $"Tenants:DefaultKey '{options.DefaultKey}' has no matching entry in Tenants:Registry.");

        // Shim substitution first — legacy fallback, default tenant only.
        var defaultEntry = options.Registry[options.DefaultKey];
        if (string.IsNullOrWhiteSpace(defaultEntry.ConnectionString))
        {
            var legacyConnectionString = configuration.GetConnectionString("SqlServerConnection");
            if (!string.IsNullOrWhiteSpace(legacyConnectionString))
                defaultEntry.ConnectionString = legacyConnectionString;
        }

        // Validation second — every entry, including default post-shim, must
        // resolve to a non-empty connection string.
        var missingConnectionStrings = options.Registry
            .Where(entry => string.IsNullOrWhiteSpace(entry.Value.ConnectionString))
            .Select(entry => entry.Key)
            .ToList();

        if (missingConnectionStrings.Count > 0)
            return ValidateOptionsResult.Fail(
                $"Tenants:Registry entries missing a connection string: {string.Join(", ", missingConnectionStrings)}.");

        return ValidateOptionsResult.Success;
    }
}
