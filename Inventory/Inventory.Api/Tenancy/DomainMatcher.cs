namespace Inventory.Api.Tenancy;

/// <summary>
/// Dot-anchored root-domain matching shared by tenant resolution and CORS origin
/// checks. Never use a raw <c>EndsWith</c> check for this purpose: it would let
/// "nottuapp.com" or "eviltuapp.com" pass as a match for root "tuapp.com" (they
/// end with the literal characters, with no separator).
/// </summary>
public static class DomainMatcher
{
    public static bool MatchesRoot(string host, string root)
    {
        return host.Equals(root, StringComparison.OrdinalIgnoreCase)
            || host.EndsWith("." + root, StringComparison.OrdinalIgnoreCase);
    }
}
