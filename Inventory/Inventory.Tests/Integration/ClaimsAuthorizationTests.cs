using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Inventory.Tests.Integration;

/// <summary>
/// Tasks 4.3, 4.5, 4.6, 4.7, 4.8 (Phase 4, Checkpoint B tests) — integration
/// coverage for the claims-authorization cutover itself: spec
/// action-code-authorization, design.md D1/D2/D5/D7/D8. Follows the same
/// <see cref="WebApplicationFactory{Program}"/> + Host header harness and
/// <see cref="TenantApiFactory.HasRealDatabases"/> self-reporting BLOCKED
/// convention already established by <see cref="TenantLoginTests"/> and
/// <see cref="UserManagementTests"/> — every permission-dependent scenario
/// here seeds a real role + RoleActions rows in the test DB rather than
/// relying on hand-built token claims to grant access, since
/// PermissionClaimsMiddleware now strips and replaces any role claims a
/// presented token carries (design.md D2).
/// </summary>
public class ClaimsAuthorizationTests : IClassFixture<TenantApiFactory>
{
    private const string DefaultTenantHost = "localhost";
    private const string DefaultTenantKey = "default";

    private readonly TenantApiFactory _factory;

    public ClaimsAuthorizationTests(TenantApiFactory factory)
    {
        _factory = factory;
    }

    // ---------------------------------------------------------------
    // 4.3 — a role holding only UsersView: 200 on reads, 403 on writes
    // ---------------------------------------------------------------

    [Fact]
    public async Task RoleWithOnlyUsersView_Returns200OnGet_403OnWrites()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var roleId = await CreateRoleAsync(client, adminToken, $"Test-UsersViewOnly-{ShortSuffix()}");
        var usersViewActionId = await GetActionIdByCodeAsync(client, adminToken, "UsersView");
        await AssignRoleActionsAsync(client, adminToken, roleId, new[] { usersViewActionId });

        var (_, userToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId);

        var getResponse = await SendWithTokenAsync(client, HttpMethod.Get, "/api/v1/users", userToken, hasBody: false);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var postResponse = await SendWithTokenAsync(client, HttpMethod.Post, "/api/v1/users", userToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, postResponse.StatusCode);

        var putResponse = await SendWithTokenAsync(client, HttpMethod.Put, "/api/v1/users/1", userToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);

        var patchResponse = await SendWithTokenAsync(client, HttpMethod.Patch, "/api/v1/users/1/status", userToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, patchResponse.StatusCode);
    }

    // ---------------------------------------------------------------
    // 4.5 — stale-token escalation (D1/D2): a pre-deploy-shaped token
    // carrying ClaimTypes.Role = "UsersView" and "SystemAdmin" must not
    // authorize anything for a user whose real DB role grants neither.
    // ---------------------------------------------------------------

    [Fact]
    public async Task StaleTokenWithRoleNameClaims_DoesNotEscalate_403OnUsersAndModulesWrites()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        // Real user with NO role at all — the DB grants nothing, so any
        // authorization must come exclusively from PermissionClaimsMiddleware
        // re-deriving claims live, never from the presented token.
        var (_, staleToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId: null,
            handBuiltRoleClaims: new[] { "UsersView", "SystemAdmin" });

        var usersWriteResponse = await SendWithTokenAsync(client, HttpMethod.Post, "/api/v1/users", staleToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, usersWriteResponse.StatusCode);

        var modulesWriteResponse = await SendWithTokenAsync(client, HttpMethod.Post, "/api/v1/modules", staleToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, modulesWriteResponse.StatusCode);
    }

    // ---------------------------------------------------------------
    // 4.6 — revoking an Action invalidates the permission cache: the next
    // request with the SAME token is rejected without re-login.
    // ---------------------------------------------------------------

    [Fact]
    public async Task RevokingAnAction_NextRequestSameToken_Returns403WithoutReLogin()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var roleId = await CreateRoleAsync(client, adminToken, $"Test-Revocable-{ShortSuffix()}");
        var usersViewActionId = await GetActionIdByCodeAsync(client, adminToken, "UsersView");
        await AssignRoleActionsAsync(client, adminToken, roleId, new[] { usersViewActionId });

        var (_, userToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId);

        var beforeRevoke = await SendWithTokenAsync(client, HttpMethod.Get, "/api/v1/users", userToken, hasBody: false);
        Assert.Equal(HttpStatusCode.OK, beforeRevoke.StatusCode);

        // Revoke every Action from the role (same endpoint, same admin token
        // — no SQL, this is the shipped role-actions-assignment surface).
        await AssignRoleActionsAsync(client, adminToken, roleId, Array.Empty<int>());

        // Same JWT, still cryptographically valid and unexpired.
        var afterRevoke = await SendWithTokenAsync(client, HttpMethod.Get, "/api/v1/users", userToken, hasBody: false);
        Assert.Equal(HttpStatusCode.Forbidden, afterRevoke.StatusCode);
    }

    // ---------------------------------------------------------------
    // 4.7 — the System Admin policy is not delegable via Action grants: a
    // non-IsSystemAdmin role holding all four Modules* Actions still gets
    // 403 on ModulesController write endpoints.
    // ---------------------------------------------------------------

    [Fact]
    public async Task RoleHoldingAllFourModulesActions_ButNotSystemAdmin_Returns403OnModulesWrites()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var roleId = await CreateRoleAsync(client, adminToken, $"Test-FakeSysAdmin-{ShortSuffix()}");
        var moduleActionIds = new List<int>
        {
            await GetActionIdByCodeAsync(client, adminToken, "ModulesView"),
            await GetActionIdByCodeAsync(client, adminToken, "ModulesCreate"),
            await GetActionIdByCodeAsync(client, adminToken, "ModulesEdit"),
            await GetActionIdByCodeAsync(client, adminToken, "ModulesDelete"),
        };
        // CreateRoleCommandHandler never sets IsSystemAdmin (design.md D1:
        // SQL-only, no command/DTO/controller may ever set it), so this role
        // holds every Modules* Action code yet Roles.IsSystemAdmin is false.
        await AssignRoleActionsAsync(client, adminToken, roleId, moduleActionIds);

        var (_, userToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId);

        var createResponse = await SendWithTokenAsync(client, HttpMethod.Post, "/api/v1/modules", userToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, createResponse.StatusCode);

        var updateResponse = await SendWithTokenAsync(client, HttpMethod.Put, "/api/v1/modules/1", userToken, hasBody: true);
        Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);
    }

    // ---------------------------------------------------------------
    // 4.8 — /auth/me for Admin: the response shape/invariants documented in
    // design.md ("UserAuthorizationData shape — roleNames, ModulePermission[]
    // — is preserved byte-for-byte; only its data source changes") must
    // still hold post-cutover. No pre-cutover JSON capture is committed in
    // this repo to diff against, so this asserts the documented invariant
    // directly: same field names, Admin's role present, and full CanView/
    // CanCreate/CanEdit/CanDelete grants on every module (04_Admin_Role.sql
    // grants Admin every seeded Action unconditionally — unchanged by this
    // cutover, only the query's data source moved from RoleModules to
    // RoleActions).
    // ---------------------------------------------------------------

    [Fact]
    public async Task AuthMe_ForAdmin_ShapeAndFullGrantsUnchangedByCutover()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);
        var adminRoleId = await db.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstAsync();
        var adminUser = await db.Users.Where(u => u.RoleId == adminRoleId && u.IsActive).FirstAsync();

        var adminToken = IssueTokenForUser(adminUser.Id, adminUser.Email);
        var client = _factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = body.RootElement.GetProperty("data");

        Assert.Equal(adminUser.Email, data.GetProperty("email").GetString());

        var roles = data.GetProperty("roles").EnumerateArray().Select(r => r.GetString()).ToList();
        Assert.Contains("Admin", roles);

        var permissions = data.GetProperty("permissions").EnumerateArray().ToList();
        Assert.NotEmpty(permissions);
        foreach (var permission in permissions)
        {
            Assert.True(permission.GetProperty("canView").GetBoolean(), $"Expected canView=true for {permission.GetProperty("module").GetString()}");
            Assert.True(permission.GetProperty("canCreate").GetBoolean(), $"Expected canCreate=true for {permission.GetProperty("module").GetString()}");
            Assert.True(permission.GetProperty("canEdit").GetBoolean(), $"Expected canEdit=true for {permission.GetProperty("module").GetString()}");
            Assert.True(permission.GetProperty("canDelete").GetBoolean(), $"Expected canDelete=true for {permission.GetProperty("module").GetString()}");
        }
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private async Task<HttpResponseMessage> SendWithTokenAsync(HttpClient client, HttpMethod method, string path, string token, bool hasBody)
    {
        var request = new HttpRequestMessage(method, path);
        if (hasBody)
        {
            request.Content = method.Method switch
            {
                "POST" when path.StartsWith("/api/v1/users") => JsonBody(new { email = $"never-{Guid.NewGuid():N}@example.com", password = "Password123!", firstName = "A", lastName = "B" }),
                "PUT" when path.StartsWith("/api/v1/users") => JsonBody(new { email = $"never-{Guid.NewGuid():N}@example.com", firstName = "A", lastName = "B" }),
                "PATCH" when path.StartsWith("/api/v1/users") => JsonBody(new { isActive = false }),
                "POST" when path.StartsWith("/api/v1/modules") => JsonBody(new { name = $"Never-{Guid.NewGuid():N}", sortOrder = 1 }),
                "PUT" when path.StartsWith("/api/v1/modules") => JsonBody(new { name = $"Never-{Guid.NewGuid():N}", sortOrder = 1 }),
                _ => null,
            };
        }

        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await client.SendAsync(request);
    }

    private async Task<int> CreateRoleAsync(HttpClient client, string adminToken, string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/roles")
        {
            Content = JsonBody(new { name }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("data").GetProperty("id").GetInt32();
    }

    private async Task<int> GetActionIdByCodeAsync(HttpClient client, string adminToken, string code)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/actions");
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var match = body.RootElement.GetProperty("data").EnumerateArray()
            .First(a => a.GetProperty("code").GetString() == code);
        return match.GetProperty("id").GetInt32();
    }

    private async Task AssignRoleActionsAsync(HttpClient client, string adminToken, int roleId, IReadOnlyList<int> actionIds)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/roles/{roleId}/actions")
        {
            Content = JsonBody(new { roleId, actionIds }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<(int UserId, string Token)> CreateActiveUserAndIssueTokenAsync(
        string adminToken, int? roleId, IReadOnlyList<string>? handBuiltRoleClaims = null)
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/users")
        {
            Content = JsonBody(new
            {
                email = $"claims-{Guid.NewGuid():N}@example.com",
                password = "Password123!",
                roleId,
                firstName = "Claims",
                lastName = "Test",
            }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = body.RootElement.GetProperty("data");
        var userId = data.GetProperty("id").GetInt32();
        var email = data.GetProperty("email").GetString()!;

        var token = handBuiltRoleClaims is null
            ? IssueTokenForUser(userId, email)
            : IssueTokenForUser(userId, email, handBuiltRoleClaims);

        return (userId, token);
    }

    private async Task<string> IssueAdminTokenAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);

        var adminRoleId = await db.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstAsync();
        var adminUser = await db.Users
            .Where(u => u.RoleId == adminRoleId && u.IsActive)
            .FirstAsync();

        return IssueTokenForUser(adminUser.Id, adminUser.Email);
    }

    /// <summary>
    /// Mints a JWT carrying only identity claims (sub/email/tenant) — no
    /// ClaimTypes.Role claims. PermissionClaimsMiddleware is the sole source
    /// of authorization claims from this checkpoint on (design.md D2), so a
    /// "real" token issued by JwtTokenGenerator never carries role names
    /// either (task 4.4: the old inert roles-claim-building loop is dropped
    /// here, not merely unused, matching JwtTokenGenerator.cs's own change).
    /// </summary>
    private string IssueTokenForUser(int userId, string email)
    {
        return BuildToken(userId, email, roleClaimValues: Array.Empty<string>());
    }

    /// <summary>
    /// Hand-builds a token carrying explicit ClaimTypes.Role claims —
    /// simulates a pre-deploy JWT minted while a tenant-created role was
    /// still literally named e.g. "UsersView" or "SystemAdmin" (design.md
    /// D1/D2's stale-token threat model). Used only by the 4.5 escalation
    /// test; every other caller in this suite goes through the real,
    /// role-claim-free <see cref="IssueTokenForUser(int, string)"/>.
    /// </summary>
    private string IssueTokenForUser(int userId, string email, IReadOnlyList<string> roleClaimValues)
    {
        return BuildToken(userId, email, roleClaimValues);
    }

    private string BuildToken(int userId, string email, IReadOnlyList<string> roleClaimValues)
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var key = configuration["Jwt:Key"]!;
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant", DefaultTenantKey),
        };
        foreach (var roleClaimValue in roleClaimValues)
            claims.Add(new Claim(ClaimTypes.Role, roleClaimValue));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Roles.Name is HasMaxLength(50) — a full 32-char GUID hex suffix
    // overflows it once combined with a descriptive prefix, so role names in
    // this suite use a short 8-char suffix instead (still unique enough for
    // parallel/re-run safety within a single test session).
    private static string ShortSuffix() => Guid.NewGuid().ToString("N")[..8];

    private static void SkipUnlessRealDatabasesConfigured()
    {
        if (!TenantApiFactory.HasRealDatabases)
        {
            throw new InvalidOperationException(
                "BLOCKED (infrastructure, not a test/code defect): this scenario requires real, " +
                "migrated, seeded per-tenant SQL Server databases (an active Admin user, per " +
                "TenantBootstrap/04-05). Set INVENTORY_TEST_DEFAULT_CONNECTION, " +
                "INVENTORY_TEST_ACME_CONNECTION, and INVENTORY_TEST_GLOBEX_CONNECTION to run it. " +
                "See proposal.md Dependencies: \"the user will create the test tenant DB, or ask for it to be created.\"");
        }
    }

    private static StringContent JsonBody(object payload) =>
        new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
}
