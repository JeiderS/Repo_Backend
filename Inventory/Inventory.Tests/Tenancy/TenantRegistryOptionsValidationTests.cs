using Inventory.Api.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace Inventory.Tests.Tenancy;

public class TenantRegistryOptionsValidationTests
{
    private static IConfiguration BuildConfiguration(string? legacyConnectionString)
    {
        var data = new Dictionary<string, string?>();
        if (legacyConnectionString is not null)
            data["ConnectionStrings:SqlServerConnection"] = legacyConnectionString;

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    [Fact]
    public void Validate_MissingDefaultKey_Fails()
    {
        var options = new TenantRegistryOptions
        {
            RootDomains = new List<string> { "localhost" },
            DefaultKey = string.Empty,
            Registry = new Dictionary<string, TenantEntryOptions>
            {
                ["default"] = new TenantEntryOptions { Name = "Mi Empresa", ConnectionString = "Server=default;" },
            },
        };

        var validator = new TenantRegistryOptionsValidator(BuildConfiguration(null));
        var result = validator.Validate(null, options);

        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_NonDefaultTenantMissingConnectionString_Fails()
    {
        var options = new TenantRegistryOptions
        {
            RootDomains = new List<string> { "localhost", "tuapp.com" },
            DefaultKey = "default",
            Registry = new Dictionary<string, TenantEntryOptions>
            {
                ["default"] = new TenantEntryOptions { Name = "Mi Empresa", ConnectionString = "Server=default;" },
                ["acme"] = new TenantEntryOptions { Name = "Acme S.A.", ConnectionString = "" },
            },
        };

        var validator = new TenantRegistryOptionsValidator(BuildConfiguration(null));
        var result = validator.Validate(null, options);

        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_EmptyDefaultConnectionString_IsShimmedFromLegacyConfigBeforeValidation_AndSucceeds()
    {
        var options = new TenantRegistryOptions
        {
            RootDomains = new List<string> { "localhost" },
            DefaultKey = "default",
            Registry = new Dictionary<string, TenantEntryOptions>
            {
                ["default"] = new TenantEntryOptions { Name = "Mi Empresa", ConnectionString = "" },
            },
        };

        var validator = new TenantRegistryOptionsValidator(BuildConfiguration("Server=legacy;"));
        var result = validator.Validate(null, options);

        Assert.False(result.Failed);
        Assert.Equal("Server=legacy;", options.Registry["default"].ConnectionString);
    }

    [Fact]
    public void Validate_EmptyDefaultConnectionStringAndNoLegacyFallback_Fails()
    {
        var options = new TenantRegistryOptions
        {
            RootDomains = new List<string> { "localhost" },
            DefaultKey = "default",
            Registry = new Dictionary<string, TenantEntryOptions>
            {
                ["default"] = new TenantEntryOptions { Name = "Mi Empresa", ConnectionString = "" },
            },
        };

        var validator = new TenantRegistryOptionsValidator(BuildConfiguration(null));
        var result = validator.Validate(null, options);

        Assert.True(result.Failed);
    }
}
