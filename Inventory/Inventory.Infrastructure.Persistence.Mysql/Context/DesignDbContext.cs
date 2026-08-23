

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Persistence.Mysql.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataBaseContext>
    {
        public DataBaseContext CreateDbContext(string[] args)
        {
            // Design-time (migrations) has no HTTP request, so there is no
            // resolved tenant. Precedence: an explicit override env var, else
            // the default tenant's configured connection string, else the
            // legacy ConnectionStrings:SqlServerConnection key.
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = Environment.GetEnvironmentVariable("INVENTORY_MIGRATION_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var defaultKey = config["Tenants:DefaultKey"];
                if (!string.IsNullOrWhiteSpace(defaultKey))
                    connectionString = config[$"Tenants:Registry:{defaultKey}:ConnectionString"];
            }

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = config.GetConnectionString("SqlServerConnection");

            var optionsBuilder = new DbContextOptionsBuilder<DataBaseContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DataBaseContext(optionsBuilder.Options);
        }
    }
}
