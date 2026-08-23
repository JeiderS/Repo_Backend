namespace Inventory.Api.Tenancy;

/// <summary>
/// Result of resolving a tenant key from a request host. Modeled as a closed
/// hierarchy (rather than a plain string + bool) so callers must handle every
/// case explicitly.
/// </summary>
public abstract record TenantResolution
{
    /// <summary>Host resolved to a known, registered tenant.</summary>
    public sealed record Resolved(string Key) : TenantResolution;

    /// <summary>Host matched a configured root domain but the remaining label
    /// has no entry in the registry (e.g. "a.b.tuapp.com" -&gt; "a.b").</summary>
    public sealed record UnknownTenant(string Key) : TenantResolution;

    /// <summary>Host does not match any configured root domain at all.</summary>
    public sealed record UnsupportedHost : TenantResolution;

    private TenantResolution() { }
}

/// <summary>
/// Pure tenant-key parsing from a request host. No <c>HttpContext</c> dependency,
/// so it is unit-testable in isolation. See design.md "root-domain-suffix
/// parsing, not first label" decision for the exact matching rule.
/// </summary>
public static class TenantResolver
{
    public static TenantResolution ResolveKey(string host, TenantRegistryOptions options)
    {
        var normalizedHost = StripPort(host).ToLowerInvariant();

        var matchedRoot = options.RootDomains
            .FirstOrDefault(root => DomainMatcher.MatchesRoot(normalizedHost, root));

        if (matchedRoot is null)
            return new TenantResolution.UnsupportedHost();

        var remainder = normalizedHost.Equals(matchedRoot, StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : normalizedHost[..^(matchedRoot.Length + 1)];

        var key = remainder.Length == 0 || remainder.Equals("www", StringComparison.OrdinalIgnoreCase)
            ? options.DefaultKey
            : remainder;

        return options.Registry.ContainsKey(key)
            ? new TenantResolution.Resolved(key)
            : new TenantResolution.UnknownTenant(key);
    }

    private static string StripPort(string host)
    {
        var colonIndex = host.LastIndexOf(':');
        return colonIndex < 0 ? host : host[..colonIndex];
    }
}
