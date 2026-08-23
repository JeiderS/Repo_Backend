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
/// Post-verify follow-up (role-permission-management coverage gap) —
/// integration coverage for the 4 role-permission-management scenarios that
/// had implementing code confirmed correct by direct read during sdd-verify
/// but no runtime-exercising test: spec role-permission-management,
/// "Admin-Only Role Creation and Edit" / "Role Action Assignment" / "Single
/// Seeded Admin Role". Follows the same <see cref="WebApplicationFactory{Program}"/>
/// + Host header harness and <see cref="TenantApiFactory.HasRealDatabases"/>
/// self-reporting BLOCKED convention already established by
/// <see cref="UserManagementTests"/> and <see cref="ClaimsAuthorizationTests"/>
/// — every permission-dependent scenario seeds a real role + RoleActions rows
/// via the live <c>PUT api/v1/roles/{id}/actions</c> endpoint rather than
/// trusting token claims, since PermissionClaimsMiddleware strips and
/// replaces any role claims a presented token carries (design.md D2).
/// </summary>
public class RoleManagementTests : IClassFixture<TenantApiFactory>
{
    private const string DefaultTenantHost = "localhost";
    private const string DefaultTenantKey = "default";

    private readonly TenantApiFactory _factory;

    public RoleManagementTests(TenantApiFactory factory)
    {
        _factory = factory;
    }

    // ---------------------------------------------------------------
    // "Requester holding RolesEdit edits an existing role" — PUT
    // api/v1/roles/{id} succeeds (200) for a requester whose role holds
    // RolesEdit, and the name change is persisted.
    // ---------------------------------------------------------------

    [Fact]
    public async Task RolesController_Update_WorksForRequesterHoldingRolesEdit()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        // Role that grants the requester RolesEdit (and nothing else).
        var editorRoleId = await CreateRoleAsync(client, adminToken, $"Test-RolesEditor-{ShortSuffix()}");
        var rolesEditActionId = await GetActionIdByCodeAsync(client, adminToken, "RolesEdit");
        await AssignRoleActionsAsync(client, adminToken, editorRoleId, new[] { rolesEditActionId });
        var (_, editorToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, editorRoleId);

        // Separate target role to be edited.
        var targetRoleId = await CreateRoleAsync(client, adminToken, $"Test-EditTarget-{ShortSuffix()}");
        var newName = $"Test-EditTarget-Renamed-{ShortSuffix()}";

        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/roles/{targetRoleId}")
        {
            Content = JsonBody(new { name = newName }),
        };
        updateRequest.Headers.Host = DefaultTenantHost;
        updateRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", editorToken);
        var updateResponse = await client.SendAsync(updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        using var updateBody = JsonDocument.Parse(await updateResponse.Content.ReadAsStringAsync());
        Assert.Equal(newName, updateBody.RootElement.GetProperty("data").GetProperty("name").GetString());

        // Confirm the rename actually persisted, not just returned in the response.
        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);
        var persistedName = await db.Roles.Where(r => r.Id == targetRoleId).Select(r => r.Name).FirstAsync();
        Assert.Equal(newName, persistedName);
    }

    // ---------------------------------------------------------------
    // "Role without the required Action is rejected" — a requester whose
    // role holds neither RolesCreate nor RolesEdit gets 403 on POST
    // api/v1/roles and PUT api/v1/roles/{id}. Mirrors
    // UserManagementTests.NonAdminAuthenticatedRequest_UsersEndpoint_Returns403
    // against the Roles endpoints.
    // ---------------------------------------------------------------

    public static IEnumerable<object[]> RolesWriteEndpoints()
    {
        yield return new object[] { "POST", "/api/v1/roles" };
        yield return new object[] { "PUT", "/api/v1/roles/{id}" };
    }

    [Theory]
    [MemberData(nameof(RolesWriteEndpoints))]
    public async Task RoleWithoutRolesCreateOrRolesEdit_RolesWriteEndpoint_Returns403(string method, string pathTemplate)
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        // Role granted no Roles* Action at all (empty RoleActions set).
        var noGrantRoleId = await CreateRoleAsync(client, adminToken, $"Test-NoRolesGrant-{ShortSuffix()}");
        await AssignRoleActionsAsync(client, adminToken, noGrantRoleId, Array.Empty<int>());
        var (_, userToken) = await CreateActiveUserAndIssueTokenAsync(adminToken, noGrantRoleId);

        // Existing target role for the PUT case (id substitution only — the
        // request must never reach the handler given the 403 gate).
        var targetRoleId = await CreateRoleAsync(client, adminToken, $"Test-PutTarget-{ShortSuffix()}");
        var path = pathTemplate.Replace("{id}", targetRoleId.ToString());

        var httpMethod = new HttpMethod(method);
        var request = new HttpRequestMessage(httpMethod, path)
        {
            Content = JsonBody(new { name = $"Never-{Guid.NewGuid():N}" }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await client.SendAsync(request);

        // Not 401 (proves authentication succeeded) and specifically 403
        // (proves the [Authorize(Roles="RolesCreate"|"RolesEdit")] gate
        // rejected a real, active principal holding neither Action).
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ---------------------------------------------------------------
    // "Assigning a non-existent action is rejected" — PUT
    // api/v1/roles/{id}/actions with an ActionId that does not exist in the
    // seeded catalog is rejected. AssignRoleActionsCommandHandler's real
    // ActionNotFound error path maps to RoleErrorBuilder.ActionNotFound(),
    // which carries HttpStatusCode.BadRequest.
    // ---------------------------------------------------------------

    [Fact]
    public async Task AssignRoleActions_WithNonExistentActionId_IsRejectedWith400()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var roleId = await CreateRoleAsync(client, adminToken, $"Test-BadAction-{ShortSuffix()}");

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/roles/{roleId}/actions")
        {
            Content = JsonBody(new { roleId, actionIds = new[] { int.MaxValue } }),
        };
        request.Headers.Host = DefaultTenantHost;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(body.RootElement.GetProperty("success").GetBoolean());
    }

    // ---------------------------------------------------------------
    // "A newly created custom role is not a system admin by default" — POST
    // api/v1/roles creates a role that has IsSystemAdmin = false, confirmed
    // directly against the DB (CreateRoleCommandHandler never sets it —
    // design.md D1, SQL-only).
    // ---------------------------------------------------------------

    [Fact]
    public async Task NewlyCreatedCustomRole_IsNotSystemAdminByDefault()
    {
        SkipUnlessRealDatabasesConfigured();

        var connectionString = Environment.GetEnvironmentVariable("INVENTORY_TEST_DEFAULT_CONNECTION")!;
        var adminToken = await IssueAdminTokenAsync(connectionString);
        var client = _factory.CreateClient();

        var roleId = await CreateRoleAsync(client, adminToken, $"Test-NotSysAdmin-{ShortSuffix()}");

        var options = new DbContextOptionsBuilder<DataBaseContext>().UseSqlServer(connectionString).Options;
        await using var db = new DataBaseContext(options);
        var isSystemAdmin = await db.Roles.Where(r => r.Id == roleId).Select(r => r.IsSystemAdmin).FirstAsync();

        Assert.False(isSystemAdmin);
    }

    // ---------------------------------------------------------------
    // Helpers — mirror ClaimsAuthorizationTests' established helpers.
    // ---------------------------------------------------------------

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

    private async Task<(int UserId, string Token)> CreateActiveUserAndIssueTokenAsync(string adminToken, int? roleId)
    {
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/users")
        {
            Content = JsonBody(new
            {
                email = $"rolemgmt-{Guid.NewGuid():N}@example.com",
                password = "Password123!",
                roleId,
                firstName = "Role",
                lastName = "Mgmt",
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

        var token = IssueTokenForUser(userId, email);
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
    /// of authorization claims from Checkpoint B on (design.md D2), matching
    /// the same role-claim-free convention already established by
    /// <see cref="ClaimsAuthorizationTests"/> and <see cref="UserManagementTests"/>.
    /// </summary>
    private string IssueTokenForUser(int userId, string email)
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
    // this suite use a short 8-char suffix instead (same convention as
    // ClaimsAuthorizationTests.ShortSuffix()).
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
