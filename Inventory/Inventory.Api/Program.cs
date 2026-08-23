
using Inventory.Application;
using Inventory.Infrastructure.Persistence.Mysql;
using Inventory.Infrastructure.Persistence.Mysql.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Inventory.Api.Auth;
using Inventory.Application.Common.Interfaces;
using Inventory.Api.Middleware;
using Inventory.Api.Tenancy;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Tenant registry: bound once at startup, restart required to add a tenant
// (design.md "IOptions bound once" decision). ValidateOnStart fails fast on
// typos/missing connection strings; the validator also runs the shim that
// fills the default tenant's connection string from the legacy
// ConnectionStrings:SqlServerConnection key before validating.
builder.Services.AddOptions<TenantRegistryOptions>()
    .Bind(builder.Configuration.GetSection(TenantRegistryOptions.SectionName))
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<TenantRegistryOptions>, TenantRegistryOptionsValidator>();

// ITenantContext is the request-scoped ambient tenant, populated by
// TenantResolutionMiddleware. TenantContext is registered directly so the
// middleware (which cannot use constructor injection for scoped services) can
// resolve and mutate the same scoped instance other consumers read through
// ITenantContext (JwtTokenGenerator, the DbContext options factory below).
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());

builder.Services.AddCors(options =>
{
    // SPA CORS ports are configurable, not hardcoded (design.md decision): an
    // origin is allowed when its host passes the same dot-anchored
    // DomainMatcher.MatchesRoot check used for tenant resolution, on a port
    // listed in Cors:SpaPorts.
    options.AddPolicy("AllowLocalhost",
        policy => policy.SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
                    return false;

                var spaPorts = builder.Configuration.GetSection("Cors:SpaPorts").Get<int[]>()
                    ?? Array.Empty<int>();
                var rootDomains = builder.Configuration.GetSection("Tenants:RootDomains").Get<string[]>()
                    ?? Array.Empty<string>();

                return spaPorts.Contains(originUri.Port)
                    && rootDomains.Any(root => DomainMatcher.MatchesRoot(originUri.Host, root));
            })
            .AllowAnyHeader()
            .AllowAnyMethod());
});


builder.Services.AddControllers();

// SystemAdminOnly gates Module management independently of any Action-code
// grant (design.md D1): holding every Modules* Action MUST NOT itself
// satisfy this policy, only Roles.IsSystemAdmin does, via the distinct
// system_admin claim PermissionClaimsMiddleware adds below.
builder.Services.AddAuthorization(options =>
    options.AddPolicy(AuthorizationPolicies.SystemAdminOnly,
        p => p.RequireClaim(PermissionClaimTypes.SystemAdmin, "true")));

builder.Services.AddRouting(routing => routing.LowercaseUrls = true);

builder.Services
    .AddApplication()
    .AddPersistence();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// IMemoryCache backs IActiveUserCache (ActiveUserValidationMiddleware) — first
// use of a cache in this codebase (design.md "Cache" decision).
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IActiveUserCache, ActiveUserCache>();

// IPermissionCache backs PermissionClaimsMiddleware (design.md D3/D9) —
// same IMemoryCache + ITenantContext composition as IActiveUserCache above.
builder.Services.AddScoped<IPermissionCache, PermissionCache>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Per-tenant DbContext: built per request from the scoped ITenantContext
// resolved by TenantResolutionMiddleware, instead of one static connection
// string bound at startup (design.md "AddDbContext with the IServiceProvider
// overload" decision). DbContextOptions default optionsLifetime is already
// Scoped, so this correctly re-resolves ITenantContext on every request.
builder.Services.AddDbContext<DataBaseContext>((sp, options) =>
    options.UseSqlServer(sp.GetRequiredService<ITenantContext>().ConnectionString));


// Configura el AddSwaggerWithJwt
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el siguiente formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });

    options.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();

app.UseCors("AllowLocalhost");

// Tenant resolution precedes authentication: an unknown/unsupported host
// fails fast (404/400) before any auth logic runs (spec: tenant-resolution,
// design.md "middleware placement and fail-fast response" decision).
app.UseTenantResolutionMiddleware();

app.UseAuthentication();

// Cross-validates the JWT tenant claim against the resolved tenant. Ships in
// the exact same deploy as the JWT claim change below — Checkpoint B is one
// atomic unit (Judgment Day round 2, CONFIRMED CRITICAL, fix-caused): a gap
// between this middleware going live and the claim shipping would 401 every
// login, not just stale sessions.
app.UseTenantClaimValidationMiddleware();

// Rejects a deactivated user's still-valid JWT before it reaches any
// controller (spec: user-administration, "Immediate Deactivation Enforcement").
// Must run after UseTenantClaimValidationMiddleware — see design.md
// "ActiveUserValidationMiddleware runs after TenantClaimValidationMiddleware"
// decision (avoids a cross-tenant user-status oracle).
app.UseActiveUserValidationMiddleware();

// Derives Action-code (+ system_admin) claims from live RoleActions/Roles
// state and replaces the identity's role-claim set with them (design.md D2).
// Must run after UseActiveUserValidationMiddleware and before
// UseAuthorization, which materializes the [Authorize] evaluation
// (design.md D8 — verified against the real pipeline order above).
app.UsePermissionClaimsMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposes the top-level-statement Program for WebApplicationFactory<Program>
// in Inventory.Tests (Checkpoint B integration tests).
public partial class Program { }
