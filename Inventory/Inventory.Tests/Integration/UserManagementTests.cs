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
/// Tasks 3.2-3.5 (Phase 3, Checkpoint B tests) — integration coverage for the
/// admin-managed user surface:
/// - spec: user-administration, "Immediate Deactivation Enforcement" (3.2)
/// - spec: user-administration, "Admin-Only User Listing" / "Single Role Per
///   User" (non-admin/anonymous gating, 3.3)
/// - spec: user-administration, "Removal of Public Self-Registration" (3.4)
/// - spec: user-administration, "Reassigning a role replaces the previous
///   one" (3.5)
///
/// Follows the same <see cref="WebApplicationFactory{Program}"/> + Host
/// header harness as <see cref="TenantLoginTests"/>. Scenarios that need a
/// real, seeded per-tenant DB (an active Admin per TenantBootstrap/04-05,
/// so a request can be minted and actually authorized end to end) use the
/// same <see cref="TenantApiFactory.HasRealDatabases"/> guard and
/// self-reporting BLOCKED pattern as TenantLoginTests — not a different
/// harness, per design.md's testing-strategy table and the apply
/// instructions for this batch.
/// </summary>
public class UserManagementTests : IClassFixture<TenantApiFactory>
{
    private const string DefaultTenantHost = "localhost";
    private const string DefaultTenantKey = "default";

    private readonly TenantApiFactory _factory;

    public UserManagementTests(TenantApiFactory factory)
    {
        _factory = factory;
    }

    // ---------------------------------------------------------------
    // 3.4 — POST api/v1/auth/register no longer exists
    // ---------------------------------------------------------------

    [Fact]
    public async Task PostAuthRegister_NoLongerExists_Returns404()
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/register")
        {
            Content = JsonBody(new { email = "whoever@example.com", password = "whatever" }),
        };
        request.Headers.Host = DefaultTenantHost;

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------------------------------------------------------------
    // 3.3 — anonymous / non-admin gating on every api/v1/users endpoint
    // ---------------------------------------------------------------

    public static IEnumerable<object[]> UsersEndpoints()
    {
        yield return new object[] { "GET", "/api/v1/users", false };
        yield return new object[] { "GET", "/api/v1/users/1", false };
        yield return new object[] { "POST", "/api/v1/users", true };
        yield return new object[] { "PUT", "/api/v1/users/1", true };
        yield return new object[] { "PATCH", "/api/v1/users/1/status", true };
    }

    [Theory]
    [MemberData(nameof(UsersEndpoints))]
    public async Task AnonymousRequest_UsersEndpoint_Returns401(string method, string path, bool hasBody)
    {
        var client = _factory.CreateClient();
        var request = BuildUsersRequest(method, path, hasBody);
        request.Headers.Host = DefaultTenantHost;

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(UsersEndpoints))]
    public async Task NonAdminAuthenticatedRequest_UsersEndpoint_Returns403(string method, string path, bool hasBody)
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var nonAdminToken = await CreateActiveUserWithoutRoleAndIssueTokenAsync(adminToken);

        var client = _factory.CreateClient();
        var request = BuildUsersRequest(method, path, hasBody);
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", nonAdminToken);

        var response = await client.SendAsync(request);

        // Not 401 (proves authentication succeeded) and specifically 403
        // (proves the [Authorize(Roles="Admin")] gate rejected a real,
        // active, non-admin principal — spec: "Single Role Per User",
        // "the user cannot access Admin-gated endpoints").
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static HttpRequestMessage BuildUsersRequest(string method, string path, bool hasBody)
    {
        var httpMethod = new HttpMethod(method);
        var request = new HttpRequestMessage(httpMethod, path);
        if (hasBody)
        {
            request.Content = method switch
            {
                "POST" => JsonBody(new { email = $"never-{Guid.NewGuid():N}@example.com", password = "Password123!", firstName = "A", lastName = "B" }),
                "PUT" => JsonBody(new { email = $"never-{Guid.NewGuid():N}@example.com", firstName = "A", lastName = "B" }),
                "PATCH" => JsonBody(new { isActive = false }),
                _ => null,
            };
        }

        return request;
    }

    // ---------------------------------------------------------------
    // 3.2 — deactivating a user immediately rejects their still-valid JWT
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeactivatedUser_StillValidJwt_IsRejectedWith403_ActiveUser_Returns200Before()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);

        var client = _factory.CreateClient();

        // Create a fresh user (IsActive=true on creation, spec:
        // CreateUserCommandHandler) and mint a token for them directly
        // (same "already-issued JWT" simulation TenantLoginTests uses),
        // so this test controls exactly when it was issued relative to
        // the deactivation below.
        var (userId, userToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId: null);

        var beforeRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        beforeRequest.Headers.Host = DefaultTenantHost;
        beforeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var beforeResponse = await client.SendAsync(beforeRequest);
        Assert.Equal(HttpStatusCode.OK, beforeResponse.StatusCode);

        // Admin deactivates the user (PATCH api/v1/users/{id}/status).
        var deactivateRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/users/{userId}/status")
        {
            Content = JsonBody(new { isActive = false }),
        };
        deactivateRequest.Headers.Host = DefaultTenantHost;
        deactivateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var deactivateResponse = await client.SendAsync(deactivateRequest);
        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);

        // Same JWT, still cryptographically valid and unexpired — must now
        // be rejected before reaching the endpoint's business logic.
        var afterRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        afterRequest.Headers.Host = DefaultTenantHost;
        afterRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var afterResponse = await client.SendAsync(afterRequest);

        Assert.Equal(HttpStatusCode.Forbidden, afterResponse.StatusCode);
        using var afterBody = JsonDocument.Parse(await afterResponse.Content.ReadAsStringAsync());
        Assert.False(afterBody.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal(403, afterBody.RootElement.GetProperty("statusCode").GetInt32());
        Assert.Contains("inactivo", afterBody.RootElement.GetProperty("message").GetString(), StringComparison.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------
    // 3.5 — assigning a second RoleId replaces the first
    // ---------------------------------------------------------------

    [Fact]
    public async Task ReassigningRoleId_ReplacesThePreviousOne()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var adminRoleId = await GetAdminRoleIdAsync(connectionString);
        var roleBId = await CreateRoleAsync(adminToken, $"Test-Role-B-{Guid.NewGuid():N}");

        // Create the user with no role, then reassign twice via PUT.
        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/users")
        {
            Content = JsonBody(new
            {
                email = $"reassign-{Guid.NewGuid():N}@example.com",
                password = "Password123!",
                firstName = "Re",
                lastName = "Assign",
            }),
        };
        createRequest.Headers.Host = DefaultTenantHost;
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var createResponse = await client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        using var createdBody = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var userId = createdBody.RootElement.GetProperty("data").GetProperty("id").GetInt32();

        // PUT #1: assign Admin's role (roleA).
        var putA = await PutUserRoleAsync(client, adminToken, userId, adminRoleId);
        Assert.Equal(HttpStatusCode.OK, putA.StatusCode);
        using var putABody = JsonDocument.Parse(await putA.Content.ReadAsStringAsync());
        Assert.Equal(adminRoleId, putABody.RootElement.GetProperty("data").GetProperty("roleId").GetInt32());

        // PUT #2: assign roleB — must REPLACE roleA, not add to it.
        var putB = await PutUserRoleAsync(client, adminToken, userId, roleBId);
        Assert.Equal(HttpStatusCode.OK, putB.StatusCode);
        using var putBBody = JsonDocument.Parse(await putB.Content.ReadAsStringAsync());
        var finalRoleId = putBBody.RootElement.GetProperty("data").GetProperty("roleId").GetInt32();

        Assert.Equal(roleBId, finalRoleId);
        Assert.NotEqual(adminRoleId, finalRoleId);

        // Confirm directly against the DB: exactly one RoleId on the scalar
        // FK column, and it is roleB, not roleA — the user "no longer holds
        // role A" (spec: "Reassigning a role replaces the previous one").
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);
        var persistedRoleId = await db.Users.Where(u => u.Id == userId).Select(u => u.RoleId).FirstAsync();
        Assert.Equal(roleBId, persistedRoleId);
    }

    private static async Task<HttpResponseMessage> PutUserRoleAsync(HttpClient client, string adminToken, int userId, int roleId)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/users/{userId}")
        {
            Content = JsonBody(new
            {
                email = $"reassign-{Guid.NewGuid():N}@example.com",
                roleId,
                firstName = "Re",
                lastName = "Assign",
            }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        return await client.SendAsync(request);
    }

    private async Task<int> CreateRoleAsync(string adminToken, string name)
    {
        var client = _factory.CreateClient();
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

    private async Task<(int UserId, string Token)> CreateActiveUserAndIssueTokenAsync(string adminToken, int? roleId)
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/users")
        {
            Content = JsonBody(new
            {
                email = $"deactivate-{Guid.NewGuid():N}@example.com",
                password = "Password123!",
                roleId,
                firstName = "Temp",
                lastName = "User",
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

        var token = IssueTokenForUser(userId, email, roles: Array.Empty<string>());
        return (userId, token);
    }

    private async Task<string> CreateActiveUserWithoutRoleAndIssueTokenAsync(string adminToken)
    {
        var (_, token) = await CreateActiveUserAndIssueTokenAsync(adminToken, roleId: null);
        return token;
    }

    private async Task<string> IssueAdminTokenAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);

        var adminRoleId = await GetAdminRoleIdAsync(connectionString);
        var adminUser = await db.Users
            .Where(u => u.RoleId == adminRoleId && u.IsActive)
            .FirstAsync();

        return IssueTokenForUser(adminUser.Id, adminUser.Email, roles: new[] { "Admin" });
    }

    private static async Task<int> GetAdminRoleIdAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);
        return await db.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstAsync();
    }

    private string IssueTokenForUser(int userId, string email, IReadOnlyList<string> roles)
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
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

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
